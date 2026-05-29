import {themes as prismThemes} from 'prism-react-renderer';
import type {Config} from '@docusaurus/types';
import type * as Preset from '@docusaurus/preset-classic';

// This runs in Node.js - Don't use client-side code here (browser APIs, JSX...)

const config: Config = {
  title: 'Vectantic',
  tagline: 'High-performance local embeddings and semantic search for .NET via ONNX.',
  favicon: 'img/favicon.ico',

  // Future flags, see https://docusaurus.io/docs/api/docusaurus-config#future
  future: {
    v4: true, // Improve compatibility with the upcoming Docusaurus v4
  },

  // Set the production url of your site here
  url: 'https://JSebas-11.github.io',
  baseUrl: '/Vectantic/',
  trailingSlash: false,

  // GitHub pages deployment config.
  // If you aren't using GitHub pages, you don't need these.
  organizationName: 'JSebas-11', // Usually your GitHub org/user name.
  projectName: 'Vectantic', // Usually your repo name.

  onBrokenLinks: 'throw',

  // Even if you don't use internationalization, you can use this field to set
  // useful metadata like html lang. For example, if your site is Chinese, you
  // may want to replace "en" with "zh-Hans".
  i18n: {
    defaultLocale: 'en',
    locales: ['en'],
  },

  presets: [
    [
      'classic',
      {
        docs: {
          sidebarPath: './sidebars.ts',
          editUrl: 'https://github.com/JSebas-11/Vectantic/tree/main/docs/',
        },
        blog: {
          showReadingTime: true,
          feedOptions: {
            type: ['rss', 'atom'],
            xslt: true,
          },
          // Please change this to your repo.
          // Remove this to remove the "edit this page" links.
          editUrl: 'https://github.com/JSebas-11/Vectantic/tree/main/docs/',
          // Useful options to enforce blogging best practices
          onInlineTags: 'warn',
          onInlineAuthors: 'warn',
          onUntruncatedBlogPosts: 'warn',
        },
        theme: {
          customCss: './src/css/custom.css',
        },
      } satisfies Preset.Options,
    ],
  ],

  themeConfig: {
    // Replace with your project's social card
    image: 'img/docusaurus-social-card.jpg',
    colorMode: { respectPrefersColorScheme: true, },
    navbar: {
      title: 'Vectantic',
      logo: {
        alt: 'Vectantic Logo',
        src: 'img/logo.svg',
      },
      items: [
        {
          type: 'docSidebar',
          sidebarId: 'docs',
          position: 'left',
          label: 'Docs',
        },
        {
          href: 'https://github.com/JSebas-11/Vectantic',
          label: 'GitHub',
          position: 'right',
        },
      ],
    },
    footer: {
      style: 'dark',
      links: [
        {
          title: 'Packages',
          items: [
            { label: 'Core', to: '/docs/core/overview' },
            { label: 'Math', to: '/docs/math/overview' },
            { label: 'Semantic', to: '/docs/semantic/overview' },
          ],
        },
        {
          title: 'SDK Links',
          items: [
            {
              label: 'GitHub',
              href: 'https://github.com/JSebas-11/Vectantic',
            },
            {
              label: 'NuGet',
              href: 'https://www.nuget.org/packages?q=vectantic',
            },
          ],
        },
        {
          title: 'Personal',
          items: [
            {
              label: 'GitHub',
              href: 'https://github.com/JSebas-11',
            },
            {
              label: 'LinkedIn',
              href: 'https://www.linkedin.com/in/jsebas11/',
            },
            {
              label: 'Instagram',
              href: 'https://www.instagram.com/juans3_11.py/',
            },
          ],
        },
      ],
      copyright: `Copyright © ${new Date().getFullYear()} Vectantic (JSebas-11). Built with Docusaurus.`,
    },
    prism: {
      theme: prismThemes.github,
      darkTheme: prismThemes.dracula,
      additionalLanguages: ['csharp', 'bash'],
    },
  } satisfies Preset.ThemeConfig,

  markdown: {
    mermaid: true,
  },

  themes: ['@docusaurus/theme-mermaid']

};

export default config;
