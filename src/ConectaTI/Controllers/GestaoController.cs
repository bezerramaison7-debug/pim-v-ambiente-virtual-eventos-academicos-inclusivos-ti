using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ConectaTI.Controllers;

public sealed record GestaoModel(Evento Evento, Atividade[] Agenda,
    (Participante Pessoa, Inscricao Inscricao)[] Inscricoes, Mensagem[] Mensagens, Auditoria[] Auditoria,
    string? Busca, Atividade? Edicao);

[Authorize(Roles = "Administrador")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class GestaoController(EventosService eventos, IConfiguration config) : Controller
{
    private const string ViewPath = "~/Views/Portal/";

    [HttpGet("/gestao")]
    [AllowAnonymous]
    public IActionResult Painel(string? busca, Guid? editar)
    {
        if (!User.IsInRole("Administrador")) return View(ViewPath + "GestaoAcesso.cshtml");
        var agenda = eventos.Agenda();
        return View(ViewPath + "Gestao.cshtml", new GestaoModel(eventos.Evento(), agenda, eventos.Inscricoes(busca),
            eventos.Mensagens(), eventos.Historico(), busca, agenda.FirstOrDefault(a => a.Id == editar)));
    }

    [HttpPost("/gestao/acesso")]
    [AllowAnonymous]
    [EnableRateLimiting("acesso")]
    public async Task<IActionResult> Acessar(string? senha)
    {
        var admin = new UsuarioAdministrador("Comissão organizadora");
        if (!admin.Autenticar(senha, config["CONECTA_ADMIN_SENHA"]))
        {
            ViewData["Erro"] = "Acesso indisponível ou senha incorreta.";
            return View(ViewPath + "GestaoAcesso.cshtml");
        }
        var identity = new ClaimsIdentity([new(ClaimTypes.Name, admin.Nome),
            new(ClaimTypes.Role, "Administrador")], CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(new ClaimsPrincipal(identity));
        return Redirect("/gestao");
    }

    [HttpPost("/gestao/evento")]
    public IActionResult Evento(string? titulo, int vagas, string? periodo, bool abertas) =>
        Alterar(() => eventos.Configurar(titulo, vagas, periodo, abertas), "Configuração atualizada.", "evento");

    [HttpPost("/gestao/atividade")]
    public IActionResult Atividade(Guid? id, string? titulo, string? tipo, string? palestrante,
        DateTime inicio, int minutos, string? descricao, string? acessibilidade, bool cancelada, string? sala) =>
        Alterar(() => eventos.SalvarAtividade(ConectaTI.Atividade.Criar(id ?? Guid.NewGuid(), titulo, tipo,
            palestrante, inicio, minutos, descricao, acessibilidade, cancelada, sala)), "Atividade salva.", "agenda");

    [HttpPost("/gestao/presenca")]
    public IActionResult Presenca(Guid id) =>
        Alterar(() => eventos.ConfirmarPresenca(id), "Presença confirmada.", "inscricoes");

    [HttpPost("/gestao/aviso")]
    public IActionResult Aviso(string? tipo, string? texto) =>
        Alterar(() => eventos.Publicar(tipo, texto), "Aviso publicado.", "avisos");

    [HttpPost("/gestao/resposta")]
    public IActionResult Resposta(Guid id, string? texto) =>
        Alterar(() => eventos.Responder(id, texto), "Resposta gravada.", "atendimento");

    private IActionResult Alterar(Action acao, string sucesso, string secao)
    {
        try
        {
            if (!ModelState.IsValid) throw new RegraException("Revise os campos informados.");
            acao();
            TempData["Sucesso"] = sucesso;
        }
        catch (RegraException ex) { TempData["Erro"] = ex.Message; }
        return Redirect("/gestao#" + secao);
    }
}
