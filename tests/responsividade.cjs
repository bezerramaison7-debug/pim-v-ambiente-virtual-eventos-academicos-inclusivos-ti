// Playwright deve estar disponível no NODE_PATH ou instalado com npm install --no-save playwright.
const assert = require('node:assert/strict');
const { chromium } = require('playwright');

async function validar() {
    const browser = await chromium.launch({
        ...(process.env.CONECTA_CHROMIUM ? { executablePath: process.env.CONECTA_CHROMIUM } : {}),
        headless: true,
    });
    const base = process.env.CONECTA_TESTE_URL || 'http://127.0.0.1:5178';
    try {
        for (const width of [320, 390, 768, 1440]) {
            const page = await browser.newPage({ viewport: { width, height: width < 700 ? 844 : 1000 } });
            for (const path of ['/', '/programacao', '/inscricao', '/participante', '/certificados', '/libras', '/gestao']) {
                const response = await page.goto(base + path);
                assert.equal(response.status(), 200, path);
                assert.equal(await page.locator('h1').count(), 1, path);
                assert.equal(await page.evaluate(() => document.documentElement.scrollWidth <= innerWidth), true, path);
                const semRotulo = await page.locator('input:not([type=hidden]), textarea, select')
                    .evaluateAll(fields => fields.some(field => field.labels.length === 0));
                assert.equal(semRotulo, false, path);
                if (path === '/' && width === 320) {
                    await page.keyboard.press('Tab');
                    assert.equal(await page.evaluate(() => document.activeElement?.getAttribute('href')), '#conteudo',
                        await page.evaluate(() => document.activeElement?.outerHTML.slice(0, 200)));
                    await page.keyboard.press('Enter');
                    assert.equal(await page.locator(':focus').getAttribute('id'), 'conteudo');
                    await page.goto(base);
                }
                if (process.env.CONECTA_CAPTURAS && path === '/' && width !== 320) {
                    await page.screenshot({ path: process.env.CONECTA_CAPTURAS + '/inicio-' + width + '.png' });
                }
            }
            console.log(`OK | ${width} px | 7 páginas, sem transbordamento, títulos e rótulos presentes`);
            await page.close();
        }
        console.log('OK | 320 px | Teclado acessa o link de salto e transfere foco ao conteúdo');
    } finally {
        await browser.close();
    }
}
validar().catch(error => {
    console.error(error);
    process.exitCode = 1;
});
