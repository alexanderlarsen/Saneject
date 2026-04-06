const GLOSSARY_TERM_SELECTOR = '.glossary-term'
const GLOSSARY_TOOLTIP_SELECTOR = '.glossary-term__tooltip'
const GLOSSARY_TOOLTIP_GUTTER_PX = 16
const GLOSSARY_TOOLTIP_GAP_REM = 0.6
const GLOSSARY_TOOLTIP_MAX_WIDTH_REM = 30

function clamp(value, min, max) {
    return Math.min(Math.max(value, min), max)
}

function getRootFontSize() {
    const rootFontSize = Number.parseFloat(getComputedStyle(document.documentElement).fontSize)
    return Number.isFinite(rootFontSize) ? rootFontSize : 16
}

function clearGlossaryTooltipLayout(term) {
    term.style.removeProperty('--glossary-tooltip-left')
    term.style.removeProperty('--glossary-tooltip-top')
    term.style.removeProperty('--glossary-tooltip-width')
}

function positionGlossaryTooltip(term) {
    const tooltip = term.querySelector(GLOSSARY_TOOLTIP_SELECTOR)
    if (!(tooltip instanceof HTMLElement)) {
        return
    }

    clearGlossaryTooltipLayout(term)

    const rootFontSize = getRootFontSize()
    const viewportWidth = document.documentElement.clientWidth
    const viewportHeight = document.documentElement.clientHeight
    const gutter = GLOSSARY_TOOLTIP_GUTTER_PX
    const preferredWidth = rootFontSize * GLOSSARY_TOOLTIP_MAX_WIDTH_REM
    const width = Math.max(0, Math.min(preferredWidth, viewportWidth - (gutter * 2)))

    term.style.setProperty('--glossary-tooltip-width', `${width}px`)

    const termRect = term.getBoundingClientRect()
    const tooltipRect = tooltip.getBoundingClientRect()
    const gap = rootFontSize * GLOSSARY_TOOLTIP_GAP_REM

    const maxLeft = Math.max(gutter, viewportWidth - gutter - tooltipRect.width)
    const naturalLeft = termRect.left
    const clampedLeft = clamp(naturalLeft, gutter, maxLeft)

    const maxTop = Math.max(gutter, viewportHeight - gutter - tooltipRect.height)
    const naturalTop = termRect.bottom + gap
    const clampedTop = clamp(naturalTop, gutter, maxTop)

    term.style.setProperty('--glossary-tooltip-left', `${clampedLeft - termRect.left}px`)
    term.style.setProperty('--glossary-tooltip-top', `${clampedTop - termRect.top}px`)
}

function initializeGlossaryTooltips() {
    const terms = Array.from(document.querySelectorAll(GLOSSARY_TERM_SELECTOR))
        .filter((term) => term instanceof HTMLElement)

    if (terms.length === 0) {
        return
    }

    const activeTerms = new Set()

    const activate = (term) => {
        activeTerms.add(term)
        positionGlossaryTooltip(term)
        requestAnimationFrame(() => {
            if (activeTerms.has(term)) {
                positionGlossaryTooltip(term)
            }
        })
    }

    const deactivate = (term) => {
        activeTerms.delete(term)
        clearGlossaryTooltipLayout(term)
    }

    for (const term of terms) {
        term.addEventListener('mouseenter', () => activate(term))
        term.addEventListener('mouseleave', () => deactivate(term))
        term.addEventListener('focusin', () => activate(term))
        term.addEventListener('focusout', (event) => {
            if (!term.contains(event.relatedTarget)) {
                deactivate(term)
            }
        })
    }

    const repositionActiveTerms = () => {
        for (const term of activeTerms) {
            positionGlossaryTooltip(term)
        }
    }

    window.addEventListener('resize', repositionActiveTerms, { passive: true })
    window.addEventListener('scroll', repositionActiveTerms, { passive: true })
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeGlossaryTooltips, { once: true })
} else {
    initializeGlossaryTooltips()
}

export default {
    iconLinks: [
        {
            icon: 'github',
            href: 'https://github.com/alexanderlarsen/Saneject',
            title: 'Saneject GitHub repo'
        },
        {
            icon: 'file-earmark-text',
            href: 'https://github.com/alexanderlarsen/Saneject/blob/main/LICENSE',
            title: 'MIT license'
        }
    ]
}
