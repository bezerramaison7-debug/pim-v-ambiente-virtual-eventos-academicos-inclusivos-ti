using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ConectaTI.Controllers;

public sealed record InicioModel(Evento Evento, Atividade[] Atividades, Aviso[] Avisos, int Vagas);
public sealed record AgendaModel(Atividade[] Atividades, string[] Tipos, string? Filtro, Aviso[] Avisos);
public sealed record InscricaoModel(Evento Evento, int Vagas, string? Nome = null, string? Email = null);
public sealed record AreaModel(Participante Pessoa, Inscricao Inscricao, Certificado? Certificado, Mensagem[] Mensagens);
public sealed record ConsultaModel(string? Codigo, Certificado? Certificado);

[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class PortalController(EventosService eventos) : Controller
{
    [HttpGet("/")]
    public IActionResult Inicio() => View(new InicioModel(eventos.Evento(), eventos.Agenda(), eventos.Avisos(), eventos.VagasRestantes()));

    [HttpGet("/programacao")]
    public IActionResult Agenda(string? tipo) => View(new AgendaModel(eventos.Agenda(tipo),
        eventos.Agenda().Select(a => a.Tipo).Distinct().Order().ToArray(), tipo, eventos.Avisos()));

    [HttpGet("/inscricao")]
    public IActionResult Inscricao() => View(new InscricaoModel(eventos.Evento(), eventos.VagasRestantes()));

    [HttpPost("/inscricao")]
    [EnableRateLimiting("acesso")]
    public async Task<IActionResult> Inscrever(string? nome, string? email)
    {
        if (User.Identity?.IsAuthenticated == true) return Redirect("/participante");
        try
        {
            var (pessoa, chave) = eventos.Inscrever(nome, email);
            await Entrar(pessoa.Id.ToString(), pessoa.Nome, "Participante");
            return View("Confirmacao", chave);
        }
        catch (RegraException ex)
        {
            ViewData["Erro"] = ex.Message;
            return View("Inscricao", new InscricaoModel(eventos.Evento(), eventos.VagasRestantes(), nome, email));
        }
    }

    [HttpGet("/participante")]
    public IActionResult Participante()
    {
        if (User.IsInRole("Administrador")) return Redirect("/gestao");
        if (!User.IsInRole("Participante")) return View("Acesso");
        try
        {
            var (pessoa, inscricao, certificado, mensagens) = eventos.Area(Id());
            return View(new AreaModel(pessoa, inscricao, certificado, mensagens));
        }
        catch (RegraException ex)
        {
            ViewData["Erro"] = ex.Message;
            return View("Acesso");
        }
    }

    [HttpPost("/participante")]
    [EnableRateLimiting("acesso")]
    public async Task<IActionResult> Acessar(string? email, string? chave)
    {
        try
        {
            var pessoa = eventos.Autenticar(email, chave) ?? throw new RegraException("E-mail ou chave incorretos.");
            await Entrar(pessoa.Id.ToString(), pessoa.Nome, "Participante");
            return Redirect("/participante");
        }
        catch (RegraException ex)
        {
            ViewData["Erro"] = ex.Message;
            return View("Acesso");
        }
    }

    [HttpPost("/participante/mensagem")]
    [Authorize(Roles = "Participante")]
    public IActionResult Mensagem(string? texto) => Alterar(() => eventos.Enviar(Id(), texto), "Mensagem enviada.");

    [HttpPost("/participante/certificado")]
    [Authorize(Roles = "Participante")]
    public IActionResult Emitir() => Alterar(() => eventos.Emitir(Id()), "Certificado disponível.");

    [HttpGet("/certificados")]
    public IActionResult Certificados(string? codigo) => View(new ConsultaModel(codigo, eventos.Consultar(codigo)));

    [HttpGet("/libras")]
    public IActionResult Libras() => View();

    [HttpPost("/sair")]
    public async Task<IActionResult> Sair()
    {
        await HttpContext.SignOutAsync();
        return Redirect("/");
    }

    [Route("/erro")]
    [IgnoreAntiforgeryToken]
    public IActionResult Erro(int? status)
    {
        ViewData["Status"] = status ?? 500;
        return View();
    }

    private Guid Id() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private async Task Entrar(string id, string nome, string papel)
    {
        var identity = new ClaimsIdentity([new(ClaimTypes.NameIdentifier, id), new(ClaimTypes.Name, nome),
            new(ClaimTypes.Role, papel)], CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(new ClaimsPrincipal(identity));
    }

    private IActionResult Alterar(Action acao, string sucesso)
    {
        try { acao(); TempData["Sucesso"] = sucesso; }
        catch (RegraException ex) { TempData["Erro"] = ex.Message; }
        return Redirect("/participante");
    }
}
