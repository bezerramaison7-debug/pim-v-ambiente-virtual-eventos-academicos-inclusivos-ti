"""Exercita as páginas e formulários em uma instância descartável do Conecta TI."""

import http.cookiejar
import os
import re
import urllib.error
import urllib.parse
import urllib.request
from uuid import uuid4

BASE = os.environ.get("CONECTA_TESTE_URL", "http://127.0.0.1:5178").rstrip("/")
SENHA = os.environ["CONECTA_ADMIN_SENHA"]
TOKEN = re.compile(r'name="__RequestVerificationToken"[^>]*value="([^"]+)"')


def browser():
    return urllib.request.build_opener(urllib.request.HTTPCookieProcessor(http.cookiejar.CookieJar()))


def get(client, path):
    with client.open(BASE + path, timeout=8) as response:
        return response.read().decode("utf-8")


def token(page):
    return TOKEN.search(page)[1]


def post(client, path, csrf, **form):
    values = urllib.parse.urlencode({**form, "__RequestVerificationToken": csrf})
    with client.open(BASE + path, values.encode(), timeout=8) as response:
        return response.read().decode("utf-8")


def check(condition, name):
    if not condition:
        raise AssertionError(name)
    print(f"OK | {name}")


def main():
    participante, gestao, publico = browser(), browser(), browser()
    check("Conecta TI" in get(publico, "/") and "Storyboards" in get(publico, "/libras"),
          "Página inicial e Libras")
    agenda = get(publico, "/programacao?tipo=Oficina")
    check("Do primeiro objeto" in agenda and "O futuro dos eventos" not in agenda, "Filtro da agenda")
    try:
        post(publico, "/inscricao", "invalido", nome="Intruso", email="nao@example.test")
    except urllib.error.HTTPError as error:
        check(error.code == 400, "Formulário rejeita token antifalsificação inválido")
    else:
        raise AssertionError("Formulário aceitou token inválido")

    nome = "Pessoa de teste " + uuid4().hex[:8]
    email = uuid4().hex + "@example.test"
    inscricao = post(participante, "/inscricao", token(get(participante, "/inscricao")), nome=nome, email=email)
    chave = re.search(r"<code>([A-F0-9]{32})</code>", inscricao)[1]
    check("Inscrição confirmada" in inscricao, "Inscrição com chave")
    repetida = post(publico, "/inscricao", token(get(publico, "/inscricao")), nome=nome, email=email)
    check("existe uma inscri" in repetida, "Bloqueio de inscrição duplicada")
    area = get(participante, "/participante")
    check("Olá" in area and nome in area, "Área privada abre na sessão da inscrição")
    try:
        anonimo_valido = token(get(publico, "/participante"))
        restrito = post(publico, "/gestao/presenca", anonimo_valido, id=uuid4().hex)
        check("Acessar minha inscri" in restrito, "Acesso anônimo à ação de gestão bloqueado")
    except urllib.error.HTTPError:
        raise AssertionError("Esperava redirecionamento para a área de acesso")
    bloqueio = post(participante, "/participante/certificado", token(area))
    check("liberado quando" in bloqueio, "Emissão bloqueada sem presença")
    envio = post(participante, "/participante/mensagem", token(bloqueio), texto="Preciso de apoio em Libras.")
    check("Mensagem enviada" in envio, "Envio de mensagem")
    admin = post(gestao, "/gestao/acesso", token(get(gestao, "/gestao")), senha=SENHA)
    check("Gestão do evento" in admin, "Autenticação da comissão")
    admin_filtrado = get(gestao, "/gestao?busca=" + urllib.parse.quote(nome))
    ids = re.findall(r'name="id" value="([^"]+)"', admin_filtrado)
    check(len(ids) >= 2 and nome in admin_filtrado, "Busca da inscrição e recebimento da mensagem")
    confirmado = post(gestao, "/gestao/presenca", token(admin_filtrado), id=ids[0])
    check("Presença confirmada" in confirmado, "Confirmação de presença")
    resposta = post(gestao, "/gestao/resposta", token(confirmado), id=ids[1], texto="Solicitação registrada em nosso atendimento.")
    aviso = post(gestao, "/gestao/aviso", token(resposta), tipo="Alteração",
                 texto="A oficina terá o material de apoio em texto.")
    check("Aviso publicado" in aviso and "material de apoio em texto" in get(publico, "/programacao"),
          "Publicação de aviso")
    saida = post(participante, "/sair", token(get(participante, "/participante")))
    check("Conecta TI" in saida, "Encerramento da sessão")
    nova_area = post(participante, "/participante", token(get(participante, "/participante")), email=email, chave=chave)
    check("Solicita" in nova_area and "em nosso atendimento" in nova_area, "Novo acesso e leitura da resposta")
    emissao = post(participante, "/participante/certificado", token(nova_area))
    codigo = re.search(r"/certificados\?codigo=([A-F0-9]{24})", emissao)[1]
    verificado = get(publico, "/certificados?codigo=" + codigo.lower())
    check(nome in verificado and "5h 0min" in verificado, "Emissão e verificação pública do certificado")
    print("Fluxo concluído sem falhas.")


if __name__ == "__main__":
    main()
