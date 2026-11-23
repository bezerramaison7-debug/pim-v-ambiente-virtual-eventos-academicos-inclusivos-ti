using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace ConectaTI;

public sealed class RegraException(string message) : Exception(message);

public static class Entrada
{
    public static string Texto(string? value, int min, int max, string label)
    {
        var text = value?.Trim() ?? "";
        if (text.Length < min || text.Length > max)
            throw new RegraException($"{label}: informe de {min} a {max} caracteres.");
        return text;
    }

    public static string Email(string? value)
    {
        var email = Texto(value, 3, 254, "E-mail").ToLowerInvariant();
        if (!MailAddress.TryCreate(email, out var parsed) || parsed.Address != email)
            throw new RegraException("Informe um e-mail válido.");
        return email;
    }
}

public sealed record Evento(string Titulo, int Vagas, string Periodo, bool InscricoesAbertas)
{
    public Evento Configurar(string? titulo, int vagas, string? periodo, bool abertas, int ocupadas)
    {
        if (vagas < Math.Max(1, ocupadas) || vagas > 5000)
            throw new RegraException("A capacidade deve comportar as inscrições existentes e ficar entre 1 e 5.000.");
        return new(Entrada.Texto(titulo, 4, 100, "Título"), vagas,
            Entrada.Texto(periodo, 4, 100, "Período"), abertas);
    }
}

public sealed record Atividade(Guid Id, string Titulo, string Tipo, string Palestrante,
    DateTime Inicio, int Minutos, string Descricao, string Acessibilidade, bool Cancelada, string? Sala = null)
{
    public static Atividade Criar(Guid id, string? titulo, string? tipo, string? palestrante,
        DateTime inicio, int minutos, string? descricao, string? acessibilidade, bool cancelada, string? sala = null)
    {
        if (inicio.Year is < 2020 or > 2100 || minutos is < 15 or > 480)
            throw new RegraException("Informe uma data válida e uma duração entre 15 e 480 minutos.");
        var endereco = sala?.Trim();
        if (!string.IsNullOrEmpty(endereco) && (endereco.Length > 500 ||
            !Uri.TryCreate(endereco, UriKind.Absolute, out var url) || url.Scheme != Uri.UriSchemeHttps ||
            string.IsNullOrEmpty(url.Host) || !string.IsNullOrEmpty(url.UserInfo)))
            throw new RegraException("Informe um endereço HTTPS válido para a sala.");
        return new(id, Entrada.Texto(titulo, 4, 100, "Título"),
            Entrada.Texto(tipo, 3, 40, "Formato"), Entrada.Texto(palestrante, 2, 80, "Convidado"),
            inicio, minutos, Entrada.Texto(descricao, 8, 500, "Descrição"),
            Entrada.Texto(acessibilidade, 4, 160, "Acessibilidade"), cancelada, endereco);
    }
}

public sealed record Participante(Guid Id, string Nome, string Email, string ChaveHash)
{
    public static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}

public sealed record UsuarioAdministrador(string Nome)
{
    public bool Autenticar(string? senha, string? configurada) =>
        !string.IsNullOrEmpty(configurada) && configurada.Length >= 12 &&
        CryptographicOperations.FixedTimeEquals(Convert.FromHexString(Participante.Hash(senha ?? "")),
            Convert.FromHexString(Participante.Hash(configurada)));
}

public sealed class Inscricao
{
    public Guid ParticipanteId { get; }
    public DateTimeOffset CriadaEm { get; }
    [JsonInclude] public bool Presente { get; private set; }

    public Inscricao(Guid participanteId, DateTimeOffset criadaEm)
    {
        ParticipanteId = participanteId;
        CriadaEm = criadaEm;
    }

    public void ConfirmarPresenca() => Presente = true;
}

public sealed record Certificado(string Codigo, Guid ParticipanteId, string Nome, string Evento,
    string Periodo, int Minutos, DateTimeOffset EmitidoEm);

public sealed record Aviso(string Tipo, string Texto, DateTimeOffset PublicadoEm);
public sealed record Mensagem(Guid Id, Guid ParticipanteId, string Texto, string? Resposta);
public sealed record Auditoria(string Acao, Guid? Referencia, DateTimeOffset Data);
