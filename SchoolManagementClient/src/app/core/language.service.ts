import { Injectable, signal } from '@angular/core';
import en from '../../assets/i18n/en.json';
import ar from '../../assets/i18n/ar.json';

export type Language = 'en' | 'ar';
interface Catalog { [key: string]: string | Catalog; }

function pairCatalogs(source: Catalog, target: Catalog, result: Record<string, string> = {}): Record<string, string> {
  for (const key of Object.keys(source)) {
    const from = source[key], to = target[key];
    if (typeof from === 'string' && typeof to === 'string') result[from] = to;
    else if (from && to && typeof from === 'object' && typeof to === 'object') pairCatalogs(from, to, result);
  }
  return result;
}

@Injectable({ providedIn: 'root' })
export class LanguageService {
  readonly language = signal<Language>(localStorage.getItem('language') === 'ar' ? 'ar' : 'en');
  private readonly dictionaries: Record<Language, Record<string, string>> = { en: pairCatalogs(en, en), ar: pairCatalogs(en, ar) };
  private readonly wordTranslations = new Map(Object.entries(this.dictionaries.ar).filter(([phrase]) => /^[A-Za-z]+$/.test(phrase)));
  private readonly observer: MutationObserver;
  private readonly originalText = new WeakMap<Text, string>();
  private readonly originalAttributes = new WeakMap<HTMLElement, Map<string, string>>();
  private readonly textNodes = new Set<Text>();
  private readonly attributeNodes = new Set<HTMLElement>();
  private applying = false;

  constructor() {
    this.apply(this.language());
    this.observer = new MutationObserver((records) => {
      if (this.applying) return;
      this.applying = true;
      for (const record of records) {
        if (record.type === 'characterData' && record.target instanceof Text) this.translateNode(record.target);
        for (const node of Array.from(record.addedNodes)) this.translateTree(node);
        if (record.type === 'attributes' && record.target instanceof HTMLElement) this.translateAttributes(record.target);
      }
      this.applying = false;
    });
    const appRoot = document.querySelector('app-root') ?? document.documentElement;
    this.observer.observe(appRoot, { subtree: true, childList: true, characterData: true, attributes: true, attributeFilter: ['aria-label', 'placeholder', 'title', 'alt'] });
    this.translateTree(appRoot);
  }
  toggle(): void { this.set(this.language() === 'en' ? 'ar' : 'en'); }
  set(language: Language): void {
    this.language.set(language);
    localStorage.setItem('language', language);
    this.apply(language);
    this.translateRoot();
  }
  text(value: string): string {
    const source = value.trim();
    if (!source) return value;
    const translated = this.dictionaries.ar[source] ?? source;
    if (this.language() === 'en') return value;
    let result = translated;
    if (result === source && /[A-Za-z]/.test(source)) result = source.replace(/[A-Za-z]+(?:['’][A-Za-z]+)?/g, (word) => this.wordTranslations.get(word) ?? word);
    return result === source ? value : value.replace(source, result);
  }
  private apply(language: Language): void {
    document.documentElement.lang = language;
    document.documentElement.dir = language === 'ar' ? 'rtl' : 'ltr';
    document.documentElement.dataset['language'] = language;
  }

  private translateTree(node: Node): void {
    if (node instanceof Text) { this.translateNode(node); return; }
    if (!(node instanceof HTMLElement || node instanceof SVGElement)) return;
    if (node instanceof HTMLElement) this.translateAttributes(node);
    node.childNodes.forEach((child) => this.translateTree(child));
  }

  private translateRoot(): void {
    for (const node of this.textNodes) {
      if (node.isConnected) this.translateNode(node);
      else this.textNodes.delete(node);
    }
    for (const node of this.attributeNodes) {
      if (node.isConnected) this.translateAttributes(node);
      else this.attributeNodes.delete(node);
    }
  }

  private translateNode(node: Text): void {
    if (node.parentElement?.closest('.language-btn')) return;
    this.textNodes.add(node);
    if (!this.originalText.has(node)) this.originalText.set(node, node.nodeValue ?? '');
    const value = this.originalText.get(node)!;
    const translated = this.text(value);
    if (node.nodeValue !== translated) node.nodeValue = translated;
  }

  private translateAttributes(node: HTMLElement): void {
    if (node.classList.contains('language-btn')) return;
    this.attributeNodes.add(node);
    let originals = this.originalAttributes.get(node);
    if (!originals) { originals = new Map(); this.originalAttributes.set(node, originals); }
    for (const attribute of ['aria-label', 'placeholder', 'title', 'alt']) {
      const value = node.getAttribute(attribute);
      if (value) {
        if (!originals.has(attribute)) originals.set(attribute, value);
        const translated = this.text(originals.get(attribute)!);
        if (translated !== value) node.setAttribute(attribute, translated);
      }
    }
  }
}
