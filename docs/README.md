# Docs

`docs/` (the pages) and `sidebars.ts` are published at <https://unambitiousfx.com/functional/> by [UnambitiousFx.github.io](https://github.com/UnambitiousFx/UnambitiousFx.github.io), which builds every UnambitiousFx library into one Docusaurus site.

- `main` is served as the latest major. Each older major is served from its latest release tag.
- Every Release run redeploys the site, so merged changes go live a few minutes later, including `docs:` commits that do not release.
- `llms.txt`, `llms-full.txt` and the custom llms files are generated under `/functional/` (configured in the site repo).

## Preview

From a clone of UnambitiousFx.github.io next to this repository:

```bash
pnpm install
FUNCTIONAL_REPO=../Functional pnpm start
```

This serves your working tree, uncommitted changes included. `pnpm build` also checks links: the site uses `onBrokenLinks: "throw"`, so a broken link here blocks the next deploy of the whole site.
