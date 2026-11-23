using System.Security.Cryptography;

namespace ConectaTI;

public sealed class EventosService(IRepositorio repositorio, TimeProvider relogio)
{
    public Evento Evento() => repositorio.Ler(e => e.Evento);
    public Atividade[] Agenda(string? tipo = null) => repositorio.Ler(e => e.Atividades
        .Where(a => string.IsNullOrEmpty(tipo) || a.Tipo == tipo).OrderBy(a => a.Inicio).ToArray());
    public Aviso[] Avisos() => repositorio.Ler(e => e.Avisos.OrderByDescending(a => a.PublicadoEm).ToArray());
    public int VagasRestantes() => repositorio.Ler(e => e.Evento.Vagas - e.Inscricoes.Count);

    public (Participante Pessoa, string Chave) Inscrever(string? nome, string? email)
    {
        var nomeValido = Entrada.Texto(nome, 2, 100, "Nome");
        var emailValido = Entrada.Email(email);
        return repositorio.Atualizar(e =>
        {
            if (!e.Evento.InscricoesAbertas) throw new RegraException("As inscrições estão encerradas.");
            if (e.Participantes.Any(p => p.Email == emailValido))
                throw new RegraException("Já existe uma inscrição com esse e-mail. Acesse a área do participante.");
            if (e.Inscricoes.Count >= e.Evento.Vagas) throw new RegraException("As vagas foram preenchidas.");
            var chave = Convert.ToHexString(RandomNumberGenerator.GetBytes(16));
            var pessoa = new Participante(Guid.NewGuid(), nomeValido, emailValido, Participante.Hash(chave));
            e.Participantes.Add(pessoa);
            e.Inscricoes.Add(new Inscricao(pessoa.Id, relogio.GetUtcNow()));
            Registrar(e, "Inscrição confirmada", pessoa.Id);
            return (pessoa, chave);
        });
    }

    public Participante? Autenticar(string? email, string? chave)
    {
        var emailValido = Entrada.Email(email);
        var codigo = chave?.Trim().ToUpperInvariant();
        if (codigo?.Length != 32) return null;
        var hash = Participante.Hash(codigo);
        return repositorio.Ler(e => e.Participantes.FirstOrDefault(p => p.Email == emailValido &&
            CryptographicOperations.FixedTimeEquals(Convert.FromHexString(p.ChaveHash), Convert.FromHexString(hash))));
    }

    public (Participante Pessoa, Inscricao Inscricao, Certificado? Certificado, Mensagem[] Mensagens) Area(Guid id) =>
        repositorio.Ler(e =>
        {
            var pessoa = e.Participantes.SingleOrDefault(p => p.Id == id)
                ?? throw new RegraException("Inscrição indisponível. Acesse novamente.");
            return (pessoa, e.Inscricoes.Single(i => i.ParticipanteId == id),
                e.Certificados.SingleOrDefault(c => c.ParticipanteId == id),
                e.Mensagens.Where(m => m.ParticipanteId == id).Reverse().ToArray());
        });

    public (Participante Pessoa, Inscricao Inscricao)[] Inscricoes(string? busca) => repositorio.Ler(e =>
        e.Participantes.Join(e.Inscricoes, p => p.Id, i => i.ParticipanteId, (p, i) => (Pessoa: p, Inscricao: i))
            .Where(x => string.IsNullOrWhiteSpace(busca) ||
                x.Pessoa.Nome.Contains(busca.Trim(), StringComparison.OrdinalIgnoreCase))
            .OrderBy(x => x.Pessoa.Nome, StringComparer.Create(new("pt-BR"), true)).ToArray());

    public void ConfirmarPresenca(Guid id) => repositorio.Atualizar(e =>
    {
        var inscricao = e.Inscricoes.SingleOrDefault(i => i.ParticipanteId == id)
            ?? throw new RegraException("Inscrição não encontrada.");
        if (!inscricao.Presente)
        {
            inscricao.ConfirmarPresenca();
            Registrar(e, "Presença confirmada", id);
        }
        return true;
    });

    public Certificado Emitir(Guid id)
    {
        var existente = repositorio.Ler(e => e.Certificados.SingleOrDefault(c => c.ParticipanteId == id));
        if (existente is not null) return existente;
        return repositorio.Atualizar(e =>
        {
            var emitido = e.Certificados.SingleOrDefault(c => c.ParticipanteId == id);
            if (emitido is not null) return emitido;
            if (!e.Inscricoes.Any(i => i.ParticipanteId == id && i.Presente))
                throw new RegraException("O certificado será liberado quando a organização confirmar sua presença.");
            var minutos = e.Atividades.Where(a => !a.Cancelada).Sum(a => a.Minutos);
            if (minutos == 0) throw new RegraException("O evento precisa ter programação ativa.");
            var pessoa = e.Participantes.Single(p => p.Id == id);
            var certificado = new Certificado(Convert.ToHexString(RandomNumberGenerator.GetBytes(12)), id,
                pessoa.Nome, e.Evento.Titulo, e.Evento.Periodo, minutos, relogio.GetUtcNow());
            e.Certificados.Add(certificado);
            Registrar(e, "Certificado emitido", id);
            return certificado;
        });
    }

    public Certificado? Consultar(string? codigo)
    {
        var busca = codigo?.Trim().ToUpperInvariant();
        return busca?.Length == 24 ? repositorio.Ler(e => e.Certificados.SingleOrDefault(c => c.Codigo == busca)) : null;
    }

    public void Configurar(string? titulo, int vagas, string? periodo, bool abertas) => repositorio.Atualizar(e =>
    {
        e.Evento = e.Evento.Configurar(titulo, vagas, periodo, abertas, e.Inscricoes.Count);
        Registrar(e, "Configuração do evento atualizada", null);
        return true;
    });

    public void SalvarAtividade(Atividade atividade) => repositorio.Atualizar(e =>
    {
        var indice = e.Atividades.FindIndex(a => a.Id == atividade.Id);
        if (indice < 0) e.Atividades.Add(atividade);
        else e.Atividades[indice] = atividade;
        Registrar(e, indice < 0 ? "Atividade criada" : "Atividade atualizada", atividade.Id);
        return true;
    });

    public void Publicar(string? tipo, string? texto) => repositorio.Atualizar(e =>
    {
        e.Avisos.Add(new Aviso(Entrada.Texto(tipo, 3, 40, "Tipo de aviso"),
            Entrada.Texto(texto, 8, 600, "Aviso"), relogio.GetUtcNow()));
        Registrar(e, "Aviso publicado", null);
        return true;
    });

    public void Enviar(Guid id, string? texto) => repositorio.Atualizar(e =>
    {
        if (!e.Inscricoes.Any(i => i.ParticipanteId == id)) throw new RegraException("Inscrição não encontrada.");
        var mensagem = new Mensagem(Guid.NewGuid(), id, Entrada.Texto(texto, 5, 600, "Mensagem"), null);
        e.Mensagens.Add(mensagem);
        Registrar(e, "Mensagem recebida", mensagem.Id);
        return true;
    });

    public void Responder(Guid id, string? texto) => repositorio.Atualizar(e =>
    {
        var indice = e.Mensagens.FindIndex(m => m.Id == id);
        if (indice < 0) throw new RegraException("Mensagem não encontrada.");
        e.Mensagens[indice] = e.Mensagens[indice] with { Resposta = Entrada.Texto(texto, 5, 600, "Resposta") };
        Registrar(e, "Mensagem respondida", id);
        return true;
    });

    public Mensagem[] Mensagens() => repositorio.Ler(e => e.Mensagens.AsEnumerable().Reverse().ToArray());
    public Auditoria[] Historico() => repositorio.Ler(e => e.Auditoria.AsEnumerable().Reverse().Take(15).ToArray());
    private void Registrar(Estado estado, string acao, Guid? id) =>
        estado.Auditoria.Add(new(acao, id, relogio.GetUtcNow()));
}
