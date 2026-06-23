import type {SidebarsConfig} from '@docusaurus/plugin-content-docs';

// This runs in Node.js - Don't use client-side code here (browser APIs, JSX...)

const sidebars: SidebarsConfig = {
  tutorialSidebar: [
    'index',
    {
      type: 'category',
      label: 'Getting Started',
      collapsed: false,
      items: ['getting-started'],
    },
    {
      type: 'category',
      label: 'Result',
      collapsed: false,
      items: ['result/index', 'result/api-reference'],
    },
    {
      type: 'category',
      label: 'Maybe',
      collapsed: false,
      items: ['maybe/index', 'maybe/api-reference'],
    },
    {
      type: 'category',
      label: 'Failures & Metadata',
      items: ['failures-and-metadata'],
    },
    {
      type: 'category',
      label: 'ASP.NET Core',
      items: [
        'aspnetcore/index',
        'aspnetcore/http-mapping',
        'aspnetcore/mvc-patterns',
        'aspnetcore/custom-mappers',
      ],
    },
    {
      type: 'category',
      label: 'Testing (xunit)',
      items: [
        'xunit/index',
        'xunit/result-assertions',
        'xunit/maybe-assertions',
        'xunit/async-assertions',
        'xunit/test-organization',
      ],
    },
    {
      type: 'category',
      label: 'Project',
      items: ['migration-from-v1', 'using-with-ai'],
    },
  ],
};

export default sidebars;
