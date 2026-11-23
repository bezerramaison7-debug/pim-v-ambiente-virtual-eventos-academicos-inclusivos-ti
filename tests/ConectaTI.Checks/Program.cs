using System.Diagnostics;
using ConectaTI;
using Microsoft.Extensions.Logging;

var diretorio = Directory.CreateTempSubdirectory("conecta-check-");
try
{
    using var logs = LoggerFactory.Create(builder => builder.AddSimpleConsole(o => o.SingleLine = true));
    var caminho = Path.Combine(diretorio.FullName, "evento.json");
    var repositorio = new RepositorioJson(caminho, logs.CreateLogger<RepositorioJson>());
    var servico = new EventosService(repositorio, TimeProvider.System);
    var checks = 0;

    void Teste(string cenario, Action verificar)
    {
        verificar();
        checks++;
        Console.WriteLine($"OK {checks:D2} | {cenario}");
    }

    Teste("Agenda inicial ordenada e filtro por formato", () =>
    {
        var agenda = servico.Agenda();
        Exigir(agenda.Length == 4 && agenda.SequenceEqual(agenda.OrderBy(a => a.Inicio)));
        Exigir(servico.Agenda("Oficina").Length == 1);
    });
    Teste("Rejeita nome vazio e e-mail inválido", () =>
    {
        Rejeitar(() => servico.Inscrever("", "teste@example.test"), "Nome");
        Rejeitar(() => servico.Inscrever("Pessoa Teste", "invalido"), "e-mail");
    });
    Teste("Inscrição normaliza e-mail e guarda apenas hash da chave", () =>
    {
        var inscrito = servico.Inscrever("Pessoa Teste", " Teste@EXAMPLE.test ");
        Exigir(inscrito.Pessoa.Email == "teste@example.test");
        Exigir(!File.ReadAllText(caminho).Contains(inscrito.Chave, StringComparison.Ordinal));
        Exigir(servico.Autenticar("teste@example.test", inscrito.Chave)?.Id == inscrito.Pessoa.Id);
        Exigir(servico.Autenticar("teste@example.test", new string('0', 32)) is null);
    });
    var pessoa = servico.Inscricoes("pESSOA").Single().Pessoa;
    Teste("Rejeita duplicata mesmo com caixa diferente", () =>
        Rejeitar(() => servico.Inscrever("Outro Nome", "TESTE@example.test"), "Já existe"));
    Teste("Rejeita emissão sem presença", () =>
        Rejeitar(() => servico.Emitir(pessoa.Id), "presença"));
    Teste("Persiste presença, emite uma vez e valida código", () =>
    {
        servico.ConfirmarPresenca(pessoa.Id);
        servico.ConfirmarPresenca(pessoa.Id);
        var reiniciado = new EventosService(new RepositorioJson(caminho, logs.CreateLogger<RepositorioJson>()), TimeProvider.System);
        Exigir(reiniciado.Area(pessoa.Id).Inscricao.Presente);
        var certificado = reiniciado.Emitir(pessoa.Id);
        Exigir(certificado.Minutos == 300 && servico.Emitir(pessoa.Id) == certificado);
        Exigir(servico.Consultar(certificado.Codigo.ToLowerInvariant()) == certificado);
        Exigir(servico.Consultar(new string('0', 24)) is null);
        Exigir(servico.Historico().Count(a => a.Acao == "Presença confirmada") == 1);
    });
    Teste("Persistência do atendimento e do aviso", () =>
    {
        servico.Enviar(pessoa.Id, "Solicito apoio acessível.");
        servico.Responder(servico.Mensagens().Single().Id, "Sua solicitação foi registrada.");
        servico.Publicar("Alteração", "Confira a nova programação.");
        Exigir(servico.Area(pessoa.Id).Mensagens.Single().Resposta == "Sua solicitação foi registrada.");
        Exigir(servico.Avisos().Any(a => a.Tipo == "Alteração"));
    });
    Teste("Inscrição encerrada impede novas vagas", () =>
    {
        servico.Configurar("Conecta TI", 2, "5 e 6 de dezembro", false);
        Rejeitar(() => servico.Inscrever("Pessoa Dois", "dois@example.test"), "encerradas");
    });
    Teste("Concorrência respeita a última vaga", () =>
    {
        servico.Configurar("Conecta TI", 2, "5 e 6 de dezembro", true);
        var aprovados = 0;
        Parallel.For(0, 8, i =>
        {
            try
            {
                servico.Inscrever($"Pessoa {i}", $"pessoa{i}@example.test");
                Interlocked.Increment(ref aprovados);
            }
            catch (RegraException ex) when (ex.Message.Contains("preenchidas", StringComparison.Ordinal)) { }
        });
        Exigir(aprovados == 1 && servico.VagasRestantes() == 0);
    });
    Teste("Cancelamento da atividade preserva certificado emitido", () =>
    {
        var existente = servico.Emitir(pessoa.Id);
        var atividade = servico.Agenda().First();
        servico.SalvarAtividade(atividade with { Cancelada = true });
        Exigir(servico.Agenda().First().Cancelada && servico.Emitir(pessoa.Id) == existente);
    });
    Teste("Arquivo corrompido gera mensagem e preserva os bytes", () =>
    {
        var corrompido = Path.Combine(diretorio.FullName, "corrompido.json");
        File.WriteAllText(corrompido, "{quebrado");
        var quebrado = new RepositorioJson(corrompido, logs.CreateLogger<RepositorioJson>());
        Rejeitar(() => quebrado.Atualizar(e => e.Evento), "indisponível");
        Exigir(File.ReadAllText(corrompido) == "{quebrado");
    });

    var amostras = new List<(double Inscricao, double Certificado)>();
    var benchmark = new EventosService(new RepositorioJson(Path.Combine(diretorio.FullName, "amostra.json"),
        logs.CreateLogger<RepositorioJson>()), TimeProvider.System);
    for (var i = 0; i < 26; i++)
    {
        var cronometro = Stopwatch.StartNew();
        var (participante, _) = benchmark.Inscrever($"Amostra {i}", $"amostra{i}@example.test");
        var inscricao = cronometro.Elapsed.TotalMilliseconds;
        benchmark.ConfirmarPresenca(participante.Id);
        cronometro.Restart();
        benchmark.Emitir(participante.Id);
        if (i > 0) amostras.Add((inscricao, cronometro.Elapsed.TotalMilliseconds));
    }
    Console.WriteLine($"Inscrição e emissão | 25 medições após aquecimento | mediana {Mediana(amostras.Select(x => x.Inscricao)):F2} ms e {Mediana(amostras.Select(x => x.Certificado)):F2} ms");
    Console.WriteLine($"{checks} cenários aprovados; 0 falhas.");
}
finally { diretorio.Delete(recursive: true); }

static double Mediana(IEnumerable<double> valores) => valores.Order().ElementAt(12);
static void Exigir(bool condicao)
{
    if (!condicao) throw new InvalidOperationException("A verificação do cenário falhou.");
}
static void Rejeitar(Action acao, string esperado)
{
    try { acao(); }
    catch (RegraException ex) when (ex.Message.Contains(esperado, StringComparison.OrdinalIgnoreCase)) { return; }
    throw new InvalidOperationException($"Rejeição esperada: {esperado}");
}
