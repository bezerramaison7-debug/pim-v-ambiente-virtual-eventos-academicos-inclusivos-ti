using System.Text.Json;

namespace ConectaTI;

public sealed class Estado
{
    public required Evento Evento { get; set; }
    public List<Atividade> Atividades { get; init; } = [];
    public List<Participante> Participantes { get; init; } = [];
    public List<Inscricao> Inscricoes { get; init; } = [];
    public List<Certificado> Certificados { get; init; } = [];
    public List<Aviso> Avisos { get; init; } = [];
    public List<Mensagem> Mensagens { get; init; } = [];
    public List<Auditoria> Auditoria { get; init; } = [];
}

public interface IRepositorio
{
    T Ler<T>(Func<Estado, T> consultar);
    T Atualizar<T>(Func<Estado, T> alterar);
}

public sealed class RepositorioJson(string arquivo, ILogger<RepositorioJson> logger) : IRepositorio
{
    private readonly object _lock = new();
    private static readonly JsonSerializerOptions Formato = new() { WriteIndented = true };

    public T Ler<T>(Func<Estado, T> consultar)
    {
        lock (_lock)
        {
            return Executar(() => consultar(Carregar()));
        }
    }

    public T Atualizar<T>(Func<Estado, T> alterar)
    {
        lock (_lock)
        {
            return Executar(() =>
            {
                var estado = Carregar();
                var resultado = alterar(estado);
                Gravar(estado);
                return resultado;
            });
        }
    }

    private Estado Carregar()
    {
        try
        {
            using var stream = File.OpenRead(arquivo);
            return JsonSerializer.Deserialize<Estado>(stream, Formato)
                ?? throw new JsonException("Estado vazio.");
        }
        catch (FileNotFoundException) { return DadosIniciais.Criar(); }
        catch (DirectoryNotFoundException) { return DadosIniciais.Criar(); }
    }

    private void Gravar(Estado estado)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(arquivo)!);
        var temporario = arquivo + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            using (var stream = new FileStream(temporario, FileMode.CreateNew, FileAccess.Write))
            {
                JsonSerializer.Serialize(stream, estado, Formato);
                stream.Flush(flushToDisk: true);
            }
            File.Move(temporario, arquivo, overwrite: true);
        }
        finally
        {
            File.Delete(temporario);
        }
    }

    private T Executar<T>(Func<T> operacao)
    {
        try { return operacao(); }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            logger.LogError(ex, "Falha no armazenamento do evento.");
            throw new RegraException("O armazenamento está indisponível. Tente novamente mais tarde.");
        }
    }
}

public static class DadosIniciais
{
    public static Estado Criar() => new()
    {
        Evento = new("Conecta TI", 120, "5 e 6 de dezembro de 2026", true),
        Atividades =
        [
            Criar("e92a4efe-07d8-495d-811e-1b61003cfb05", 5, 9, 0, 60, "Abertura", "Tecnologia que inclui", "Comissão Conecta TI",
                "Boas-vindas e conversa sobre barreiras de acesso em eventos de tecnologia.", "Libras e legendas previstas"),
            Criar("e92a4efe-07d8-495d-811e-1b61003cfb06", 5, 10, 30, 90, "Minicurso", "Interfaces para todas as pessoas", "Camila Rocha",
                "Prática de HTML semântico, navegação por teclado e contraste.", "Libras e material em texto previstos"),
            Criar("e92a4efe-07d8-495d-811e-1b61003cfb07", 6, 9, 0, 90, "Oficina", "Do primeiro objeto ao C#", "Rafael Mendes",
                "Exercício guiado de classes, regras de negócio e persistência.", "Libras e roteiro em texto previstos"),
            Criar("e92a4efe-07d8-495d-811e-1b61003cfb08", 6, 11, 0, 60, "Mesa-redonda", "O futuro dos eventos inclusivos", "Equipe convidada",
                "Discussão sobre participação, comunicação bilíngue e melhorias para a próxima edição.", "Libras e chat em texto previstos")
        ],
        Avisos = [new("Inscrições", "Inscrições abertas para a edição demonstrativa do Conecta TI.",
            new DateTimeOffset(2026, 11, 23, 9, 0, 0, TimeSpan.FromHours(-3)))]
    };

    private static Atividade Criar(string id, int dia, int hora, int minuto, int duracao,
        string tipo, string titulo, string palestrante, string descricao, string acesso) =>
        Atividade.Criar(Guid.Parse(id), titulo, tipo, palestrante,
            new DateTime(2026, 12, dia, hora, minuto, 0), duracao, descricao, acesso, false);
}
