# Disseny Responsive - Aplicació Escoles Públiques

## Resum de canvis implementats

Aquesta aplicació web ha estat completament optimitzada per ser **100% responsive** i compatible amb tots els dispositius i navegadors moderns.

## Característiques Responsive Implementades

### 📱 Breakpoints i Media Queries

- **Mòbil petit**: `< 576px` - Disseny optimitzat per pantalles petites
- **Mòbil**: `< 768px` - Layout d'una columna, menu hamburguesa
- **Tablet**: `768px - 992px` - Grid de 2 columnes
- **Desktop**: `> 992px` - Grid de 4 columnes, layout complet

### 🎯 Components Responsives

#### 1. **Navegació**
- Menu hamburguesa automàtic en pantalles petites (< 768px)
- Botó `navbar-toggler` de Bootstrap 5
- Menu vertical en mòbil amb indicador d'estat actiu
- Links amb alçada mínima de 44px per dispositius tàctils

#### 2. **Taules**
- Wrapper `.table-responsive` per scroll horizontal
- Transformació a **cards en mòbil** amb atributs `data-label`
- Headers ocults en mòbil, informació mostrada com a llista
- Aplicat a totes les vistes: Schools, Students, Enrollments, AnnualFees

#### 3. **Grid d'Escoles (Scopes)**
```css
/* Desktop: 4 columnes */
grid-template-columns: repeat(4, 1fr);

/* Tablet: 2 columnes */
@media (max-width: 991.98px) {
    grid-template-columns: repeat(2, 1fr);
}

/* Mòbil: 1 columna */
@media (max-width: 767.98px) {
    grid-template-columns: 1fr;
}
```

#### 4. **Hero Section**
- Títol amb mides adaptatives (1.75rem → 2rem → 2.5rem → 3.5rem)
- Padding ajustat per cada mida de pantalla
- Formulari de cerca amb input responsive

### 🔧 Optimitzacions Tècniques

#### Cross-Browser Compatibility
- Prefixos CSS per compatibilitat: `-webkit-`, `-moz-`, `-ms-`, `-o-`
- Fixes específics per Safari amb `@supports (-webkit-appearance: none)`
- `backface-visibility` per millor rendiment en animacions

#### Dispositius Tàctils
```css
@media (hover: none) and (pointer: coarse) {
    /* Elements amb alçada mínima de 44px (recomanació Apple/Google) */
    .btn { min-height: 44px; min-width: 44px; }
    
    /* Tap highlight per millor feedback */
    a, button { -webkit-tap-highlight-color: rgba(194, 0, 0, 0.1); }
}
```

#### Mode Paisatge
- Padding reduït en hero-section
- Títols més petits
- Grid items amb alçada mínima ajustada

#### Accessibilitat
- **High Contrast Mode**: Borders més prominents
- **Reduced Motion**: Transicions desactivades per usuaris amb preferències d'accessibilitat
- **Print Styles**: Elements no essencials ocults a la impressió

### 📐 Viewport Configuration
```html
<meta name="viewport" content="width=device-width, initial-scale=1.0" />
```

### 🎨 CSS Custom Properties (Variables)
Mantinguts els colors DavidGov originals:
```css
--davidgov-red: #C20000;
--davidgov-dark: #1a1a1a;
```

## Proves Recomanades

### Dispositius
- ✅ iPhone SE (375px)
- ✅ iPhone 12 Pro (390px)
- ✅ Samsung Galaxy (360px)
- ✅ iPad (768px)
- ✅ iPad Pro (1024px)
- ✅ Desktop (1920px)

### Navegadors
- ✅ Chrome (últimes versions)
- ✅ Firefox (últimes versions)
- ✅ Safari (iOS i macOS)
- ✅ Edge (últimes versions)
- ✅ Samsung Internet

### Orientacions
- ✅ Portrait (vertical)
- ✅ Landscape (horitzontal)

## Eines de Desenvolupament

### Chrome DevTools
1. F12 → Toggle device toolbar (Ctrl+Shift+M)
2. Seleccionar dispositiu o mida personalitzada
3. Provar diferents orientacions i zoom

### Firefox Responsive Design Mode
1. F12 → Responsive Design Mode (Ctrl+Shift+M)
2. Provar diferents mides de pantalla

## Fitxers Modificats

### CSS
- `wwwroot/css/davidgov-theme.css` - Media queries, responsive grid, taules, accessibilitat

### Views
- `Views/Shared/_Layout.cshtml` - Navbar hamburguesa, viewport
- `Views/Schools/Index.cshtml` - Taula responsive amb data-labels
- `Views/Students/Index.cshtml` - Taula responsive amb data-labels
- `Views/Enrollments/Index.cshtml` - Taula responsive amb data-labels
- `Views/AnnualFees/Index.cshtml` - Taula responsive amb data-labels
- `Views/Home/Index.cshtml` - Grid responsive amb col-12 col-md-6
- `Views/Search/_SearchBar.cshtml` - Eliminat wrapper redundant

## Bones Pràctiques Implementades

1. **Mobile First**: CSS base per mòbil, media queries per pantalles més grans
2. **Touch Targets**: Mínims 44x44px per elements interactius
3. **Flexible Images**: Mai excedeixen el contenidor pare
4. **Readable Text**: Mida de font mínima de 0.875rem en mòbil
5. **No Horizontal Scroll**: Excepte taules amb `.table-responsive`
6. **Performance**: Transicions amb `transform` enlloc de `top/left`
7. **Semantic HTML**: Estructura correcta d'encapçalaments
8. **Bootstrap Grid**: Utilització de classes col-* per layouts flexibles

## Suport de Navegadors

| Navegador | Versió Mínima | Suport |
|-----------|---------------|---------|
| Chrome | 90+ | ✅ Complet |
| Firefox | 88+ | ✅ Complet |
| Safari | 14+ | ✅ Complet |
| Edge | 90+ | ✅ Complet |
| Samsung Internet | 14+ | ✅ Complet |
| Opera | 76+ | ✅ Complet |

## Manteniment

Per afegir nous components responsive:

1. Utilitza classes Bootstrap 5: `col-12 col-md-6 col-lg-4`
2. Afegeix data-labels a les taules: `<td data-label="Nom">valor</td>`
3. Wrapper `.table-responsive` per taules noves
4. Min-height 44px per elements tàctils
5. Prova en mòbil abans de fer commit

## Futur

Possibles millores:

- [ ] Progressive Web App (PWA)
- [ ] Service Worker per offline
- [ ] Dark mode amb `prefers-color-scheme`
- [ ] Lazy loading d'imatges
- [ ] Virtual scrolling per llistes llargues
- [ ] Skeleton loaders durant càrregues

---

**Data d'implementació**: Gener 2025  
**Framework**: ASP.NET Core 8.0 + Bootstrap 5  
**Responsable**: Clean Architecture Pattern
