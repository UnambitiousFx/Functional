import { themes as prismThemes } from "prism-react-renderer";
import type { Config } from "@docusaurus/types";
import type * as Preset from "@docusaurus/preset-classic";

const config: Config = {
  title: "Functional - UnambitiousFx",
  tagline:
    "Result, Maybe, and railway-oriented error handling for .NET — zero-allocation and Native-AOT ready.",
  favicon: "img/favicon.ico",

  headTags: [
    {
      tagName: "link",
      attributes: {
        rel: "icon",
        type: "image/svg+xml",
        href: "/img/favicon.svg",
      },
    },
    {
      tagName: "link",
      attributes: {
        rel: "apple-touch-icon",
        href: "/img/apple-touch-icon.png",
      },
    },
  ],

  future: {
    v4: true, // Improve compatibility with the upcoming Docusaurus v4
  },

  url: "https://functional.unambitiousfx.com",
  baseUrl: "/",

  organizationName: "UnambitiousFx",
  projectName: "Functional",

  plugins: [
    [
      "docusaurus-plugin-llms",
      {
        generateLLMsTxt: true,
        generateLLMsFullTxt: true,
        excludeImports: true,
        removeDuplicateHeadings: true,
        includeOrder: [
          "index.mdx",
          "getting-started.mdx",
          "result/index.mdx",
          "result/api-reference.mdx",
          "maybe/index.mdx",
          "maybe/api-reference.mdx",
          "failures-and-metadata.mdx",
          "aspnetcore/index.mdx",
          "xunit/index.mdx",
        ],
        includeUnmatchedLast: true,
        customLLMFiles: [
          {
            filename: "llms-result.txt",
            includePatterns: [
              "result/**",
              "maybe/**",
              "failures-and-metadata.mdx",
            ],
            fullContent: true,
            title: "Functional — Result, Maybe & Failures",
            description:
              "The Result and Maybe types, railway-oriented composition, and the failure model.",
          },
          {
            filename: "llms-testing.txt",
            includePatterns: ["xunit/**"],
            fullContent: true,
            title: "Functional — Testing with Functional.xunit",
            description:
              "Fluent assertions for Result and Maybe in xUnit tests.",
          },
        ],
      },
    ],
  ],

  themes: ["@docusaurus/theme-mermaid"],

  markdown: {
    mermaid: true,
  },

  onBrokenLinks: "throw",

  i18n: {
    defaultLocale: "en",
    locales: ["en"],
  },

  presets: [
    [
      "classic",
      {
        docs: {
          sidebarPath: "./sidebars.ts",
          editUrl: "https://github.com/UnambitiousFx/Functional/",
        },
        blog: false,
        theme: {
          customCss: "./src/css/custom.css",
        },
      } satisfies Preset.Options,
    ],
  ],

  themeConfig: {
    image: "img/unambitiousfx-social-card.png",
    colorMode: {
      respectPrefersColorScheme: true,
    },
    navbar: {
      title: "Functional",
      logo: {
        alt: "UnambitiousFx Functional Logo",
        src: "img/unambitiousfx-icon-light.svg",
        srcDark: "img/unambitiousfx-icon-dark.svg",
      },
      items: [
        {
          type: "docSidebar",
          sidebarId: "tutorialSidebar",
          position: "left",
          label: "Docs",
        },
        {
          type: "doc",
          docId: "result/index",
          position: "left",
          label: "Result",
        },
        {
          type: "doc",
          docId: "maybe/index",
          position: "left",
          label: "Maybe",
        },
        {
          type: "doc",
          docId: "aspnetcore/index",
          position: "left",
          label: "ASP.NET Core",
        },
        {
          href: "https://www.nuget.org/packages?q=UnambitiousFx.Functional",
          label: "NuGet",
          position: "right",
        },
        {
          href: "https://github.com/UnambitiousFx/Functional",
          label: "GitHub",
          position: "right",
        },
      ],
    },
    footer: {
      style: "dark",
      links: [
        {
          title: "Docs",
          items: [
            {
              label: "Getting Started",
              to: "/docs/getting-started",
            },
            {
              label: "Result",
              to: "/docs/result/",
            },
            {
              label: "Maybe",
              to: "/docs/maybe/",
            },
            {
              label: "Testing",
              to: "/docs/xunit/",
            },
          ],
        },
        {
          title: "More",
          items: [
            {
              label: "GitHub",
              href: "https://github.com/UnambitiousFx/Functional",
            },
            {
              label: "NuGet",
              href: "https://www.nuget.org/packages?q=UnambitiousFx.Functional",
            },
          ],
        },
      ],
      copyright: `Copyright © ${new Date().getFullYear()} UnambitiousFx. Built with Docusaurus.`,
    },
    prism: {
      theme: prismThemes.github,
      darkTheme: prismThemes.dracula,
      additionalLanguages: ["csharp"],
    },
    algolia: {
      // The application ID provided by Algolia
      appId: "7E7V89X5BP",

      // Public API key: it is safe to commit it
      apiKey: "74e0673104a4ecb4fe9ffed3d4601873",

      indexName: "Unambitious - Functional",

      // Optional: see doc section below
      contextualSearch: true,

      // Optional: Specify domains where the navigation should occur through window.location instead on history.push. Useful when our Algolia config crawls multiple documentation sites and we want to navigate with window.location.href to them.
      externalUrlRegex: "external\\.com|domain\\.com",

      // Optional: Algolia search parameters
      searchParameters: {},

      // Optional: path for search page that enabled by default (`false` to disable it)
      searchPagePath: "search",

      // Optional: whether the insights feature is enabled or not on Docsearch (`false` by default)
      insights: false,

      // Optional: whether you want to use the new Ask AI feature (undefined by default)
      askAi: "YOUR_ALGOLIA_ASK_AI_ASSISTANT_ID",

      //... other Algolia params
    },
  } satisfies Preset.ThemeConfig,
};

export default config;
