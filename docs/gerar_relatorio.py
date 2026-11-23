"""Gera o DOCX do relatório acadêmico. Requer python-docx."""

import re
from pathlib import Path

from docx import Document
from docx.enum.section import WD_SECTION
from docx.enum.style import WD_STYLE_TYPE
from docx.enum.text import WD_ALIGN_PARAGRAPH, WD_TAB_ALIGNMENT, WD_TAB_LEADER
from docx.oxml import OxmlElement
from docx.oxml.ns import qn
from docx.shared import Cm, Pt, RGBColor

PASTA = Path(__file__).resolve().parent
TITULO = "AMBIENTE VIRTUAL RESPONSIVO PARA EVENTOS ACADÊMICOS INCLUSIVOS EM TI"
AUTORIA = "[NOME COMPLETO E RA DO AUTOR OU DOS INTEGRANTES]"
CIDADE = "[CIDADE DO POLO]"
ANO = "2026"
TITULOS_REFERENCIAS = (
    "Lei nº 10.436, de 24 de abril de 2002",
    "Decreto nº 5.626, de 22 de dezembro de 2005",
    "Getting to yes: negotiating agreement without giving in",
    "Dicionário Libras",
    "Desenvolvendo aplicativos MVC do ASP.NET Core",
    "Manual do PIM V",
    "Web Content Accessibility Guidelines (WCAG) 2.2",
)

RESUMO = (
    "O trabalho apresenta o Conecta TI, um protótipo responsivo para planejamento e gestão de uma edição "
    "acadêmica inclusiva em Tecnologia da Informação. O ambiente integra programação, inscrição, comunicação "
    "com participantes, registro de presença e emissão de certificado com verificação por código. "
    "A aplicação foi desenvolvida em HTML5 e CSS3, com ASP.NET Core MVC, regras de negócio em C# e "
    "persistência em JSON. Foram elaborados um plano de comunicação, liderança e negociação, um glossário "
    "contextual e três storyboards de orientações em Língua Brasileira de Sinais. A avaliação reuniu "
    "verificações de domínio e um percurso HTTP entre visitante, participante e comissão. Os testes "
    "confirmaram a consistência das operações demonstradas, incluindo a reserva da última vaga, a restrição "
    "de emissão pela presença e a consulta pública do certificado. O protótipo destina-se à avaliação "
    "acadêmica em instância única. A realização de uma edição com público exige a produção e a validação "
    "dos recursos em Libras e dos serviços de acessibilidade previstos."
)
ABSTRACT = (
    "This study presents Conecta TI, a responsive prototype for planning and managing an inclusive "
    "academic event in information technology. The platform integrates the schedule, registration, "
    "participant communication, attendance confirmation, and certificate issuance with verification codes. "
    "The application uses HTML5 and CSS3, ASP.NET Core MVC, C# business rules, and JSON persistence. "
    "Deliverables include a communication, leadership, and negotiation plan, a contextual glossary, "
    "and three storyboards for guidance in Brazilian Sign Language. The evaluation combined domain checks "
    "with an HTTP workflow involving visitors, participants, and organizers. Tests confirmed the "
    "demonstrated operations, including reservation of the last available seat, certificate eligibility "
    "based on attendance, and public certificate verification. The prototype is intended for academic "
    "assessment in a single-instance environment. A live edition requires the production and validation "
    "of the sign language materials and planned accessibility services."
)


def ajustar_pagina(section):
    section.page_width, section.page_height = Cm(21), Cm(29.7)
    section.top_margin = section.left_margin = Cm(3)
    section.bottom_margin = section.right_margin = Cm(2)
    section.header_distance = Cm(1.7)


def configurar_estilos(doc):
    normal = doc.styles["Normal"]
    normal.font.name = "Times New Roman"
    normal.font.size = Pt(12)
    normal.font.color.rgb = RGBColor(0, 0, 0)
    normal.paragraph_format.alignment = WD_ALIGN_PARAGRAPH.JUSTIFY
    normal.paragraph_format.first_line_indent = Cm(1.5)
    normal.paragraph_format.line_spacing = 1.5
    normal.paragraph_format.space_before = Pt(0)
    normal.paragraph_format.space_after = Pt(0)

    for nome in ("Title", "Heading 1", "Heading 2"):
        estilo = doc.styles[nome]
        estilo.font.name = "Times New Roman"
        for borda in estilo.element.xpath("./w:pPr/w:pBdr"):
            borda.getparent().remove(borda)
        estilo.font.size = Pt(12)
        estilo.font.bold = True
        estilo.font.color.rgb = RGBColor(0, 0, 0)
        formato = estilo.paragraph_format
        formato.first_line_indent = Cm(0)
        formato.alignment = WD_ALIGN_PARAGRAPH.LEFT
        formato.space_before = Pt(0)
        formato.space_after = Pt(14)
        formato.line_spacing = 1.5

    simples = doc.styles.add_style("Texto simples", WD_STYLE_TYPE.PARAGRAPH)
    simples.base_style = normal
    simples.paragraph_format.first_line_indent = Cm(0)
    simples.paragraph_format.line_spacing = 1
    simples.paragraph_format.space_after = Pt(12)


def centralizar(doc, texto, antes=0, depois=0, estilo="Normal"):
    p = doc.add_paragraph(texto, style=estilo)
    f = p.paragraph_format
    f.alignment = WD_ALIGN_PARAGRAPH.CENTER
    f.first_line_indent = Cm(0)
    f.space_before = Pt(antes)
    f.space_after = Pt(depois)
    return p


def numerar(section):
    section.header.is_linked_to_previous = False
    section.footer.is_linked_to_previous = False
    numbering = OxmlElement("w:pgNumType")
    numbering.set(qn("w:start"), "5")
    section._sectPr.append(numbering)
    header = section.header.paragraphs[0]
    header.alignment = WD_ALIGN_PARAGRAPH.RIGHT
    header.paragraph_format.first_line_indent = Cm(0)
    field = OxmlElement("w:fldSimple")
    field.set(qn("w:instr"), "PAGE")
    header._p.append(field)


def adicionar_identificacao(doc):
    centralizar(doc, "UNIVERSIDADE PAULISTA")
    centralizar(doc, "CURSO SUPERIOR DE TECNOLOGIA EM ANÁLISE E DESENVOLVIMENTO DE SISTEMAS")
    centralizar(doc, AUTORIA, antes=54)
    centralizar(doc, TITULO, antes=125, estilo="Title")
    centralizar(doc, "Conecta TI", antes=6)
    centralizar(doc, CIDADE, antes=211)
    centralizar(doc, ANO)
    doc.add_page_break()
    centralizar(doc, AUTORIA)
    centralizar(doc, TITULO, antes=137, estilo="Title")
    centralizar(doc, "Conecta TI", antes=6)
    p = doc.add_paragraph(
        "Projeto Integrado Multidisciplinar V apresentado ao Curso Superior de Tecnologia em Análise "
        "e Desenvolvimento de Sistemas da Universidade Paulista como atividade de avaliação.",
        style="Texto simples",
    )
    p.paragraph_format.left_indent = Cm(8)
    p.paragraph_format.space_before = Pt(56)
    o = doc.add_paragraph("Orientação: [NOME DO(A) ORIENTADOR(A), SE APLICÁVEL]", style="Texto simples")
    o.paragraph_format.left_indent = Cm(8)
    o.alignment = WD_ALIGN_PARAGRAPH.LEFT
    centralizar(doc, CIDADE, antes=122)
    centralizar(doc, ANO)
    doc.add_page_break()
    centralizar(doc, "RESUMO", depois=20, estilo="Heading 1")
    doc.add_paragraph(RESUMO, style="Texto simples")
    doc.add_paragraph("Palavras-chave: eventos acadêmicos. acessibilidade digital. Libras. C#.", style="Texto simples")
    doc.add_page_break()
    centralizar(doc, "ABSTRACT", depois=20, estilo="Heading 1")
    doc.add_paragraph(ABSTRACT, style="Texto simples")
    doc.add_paragraph("Keywords: academic events. digital accessibility. Brazilian Sign Language. C#.", style="Texto simples")
    doc.add_page_break()


def adicionar_sumario(doc, paginas):
    centralizar(doc, "SUMÁRIO", depois=16, estilo="Heading 1")
    for numero, pagina in enumerate(paginas, start=5):
        for linha in pagina.splitlines():
            if not linha.startswith("#"):
                break
            titulo = linha.lstrip("# ").strip()
            nivel = 2 if linha.startswith("## ") or re.match(r"^\d+\.\d", titulo) else 1
            p = doc.add_paragraph(style="Texto simples")
            formato = p.paragraph_format
            formato.left_indent = Cm(.55 if nivel == 2 else 0)
            formato.space_after = Pt(4)
            formato.tab_stops.add_tab_stop(Cm(16), WD_TAB_ALIGNMENT.RIGHT, WD_TAB_LEADER.DOTS)
            p.add_run(titulo + "\t" + str(numero)).bold = nivel == 1


def adicionar_corpo(doc, paginas):
    for indice, pagina in enumerate(paginas):
        if indice:
            doc.add_page_break()
        referencias = pagina.startswith("# REFERÊNCIAS")
        for bloco in pagina.split("\n\n"):
            if bloco.startswith("#"):
                for linha in bloco.splitlines():
                    texto = linha.lstrip("# ").strip()
                    nivel = 2 if linha.startswith("## ") or re.match(r"^\d+\.\d", texto) else 1
                    doc.add_heading(texto, level=nivel)
            else:
                texto = bloco.replace("\n", " ")
                if referencias:
                    p = doc.add_paragraph(style="Texto simples")
                    p.alignment = WD_ALIGN_PARAGRAPH.LEFT
                    titulo = next(t for t in TITULOS_REFERENCIAS if t in texto)
                    antes, _, depois = texto.partition(titulo)
                    p.add_run(antes)
                    p.add_run(titulo).italic = True
                    p.add_run(depois)
                else:
                    doc.add_paragraph(texto)


def main():
    fonte = (PASTA / "relatorio-base.md").read_text(encoding="utf-8").strip()
    paginas = re.split(r"\n\n(?=# )", fonte)
    assert len(paginas) == 20
    doc = Document()
    doc.core_properties.title = TITULO.title()
    doc.core_properties.subject = "Projeto Integrado Multidisciplinar V"
    doc.core_properties.author = ""
    ajustar_pagina(doc.sections[0])
    configurar_estilos(doc)
    adicionar_identificacao(doc)
    adicionar_sumario(doc, paginas)
    numerar(doc.add_section(WD_SECTION.NEW_PAGE))
    adicionar_corpo(doc, paginas)
    doc.save(PASTA / "PIM_V_Conecta_TI.docx")


if __name__ == "__main__":
    main()
