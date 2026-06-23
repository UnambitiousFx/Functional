import type {ReactNode} from 'react';
import clsx from 'clsx';
import Link from '@docusaurus/Link';
import useDocusaurusContext from '@docusaurus/useDocusaurusContext';
import Layout from '@theme/Layout';
import Heading from '@theme/Heading';
import HomepageFeatures from '@site/src/components/HomepageFeatures';

import styles from './index.module.css';

const QUICK_START = `// Compose with railway-oriented programming
public Result<Order> PlaceOrder(OrderRequest request) =>
    Validate(request)               // Result<OrderRequest>
        .Bind(LoadCustomer)         // Result<Customer>
        .Ensure(c => c.IsActive,
            new ValidationFailure("Customer is inactive"))
        .Map(CreateOrder)           // Result<Order>
        .Tap(order => _log.Created(order.Id));

// Pattern match — errors are values, not exceptions
var response = PlaceOrder(request).Match(
    success: order => Results.Ok(order),
    failure: error => Results.BadRequest(error.Message));

// Optionals without null
Maybe<User> user = _users.TryFind(id);
string name = user
    .Map(u => u.Name)
    .ValueOr("anonymous");`;

function HomepageHeader() {
  const {siteConfig} = useDocusaurusContext();
  return (
    <header className={styles.heroBanner}>
      <div className={clsx('container', styles.heroInner)}>
        <div className={styles.heroLeft}>
          <Heading as="h1" className="hero__title" style={{color: 'inherit', margin: 0}}>
            {siteConfig.title}
          </Heading>
          <p className={styles.heroTagline}>{siteConfig.tagline}</p>
          <div className={styles.heroButtons}>
            <Link
              className="button button--primary button--lg"
              to="/docs/getting-started">
              Get Started →
            </Link>
            <Link
              className="button button--outline button--lg"
              style={{color: 'inherit', borderColor: 'rgba(255,255,255,0.4)'}}
              href="https://github.com/UnambitiousFx/Functional">
              View on GitHub
            </Link>
          </div>
          <div className={styles.heroInstall}>
            <code>dotnet add package UnambitiousFx.Functional</code>
          </div>
        </div>

        <div className={styles.heroCode}>
          <div className={styles.heroCodeLabel}>Quick start</div>
          <pre>{QUICK_START}</pre>
        </div>
      </div>
    </header>
  );
}

type NextStepItem = {
  emoji: string;
  title: string;
  description: string;
  to: string;
};

const NEXT_STEPS: NextStepItem[] = [
  {
    emoji: '🚀',
    title: 'Getting Started',
    description: 'Install the package and write your first Result.',
    to: '/docs/getting-started',
  },
  {
    emoji: '🚂',
    title: 'Result',
    description: 'Railway-oriented composition: Bind, Map, Ensure, Recover.',
    to: '/docs/result/',
  },
  {
    emoji: '🎁',
    title: 'Maybe',
    description: 'Optional values without null — Some, None, and friends.',
    to: '/docs/maybe/',
  },
  {
    emoji: '🧪',
    title: 'Testing',
    description: 'Fluent assertions for Result and Maybe with Functional.xunit.',
    to: '/docs/xunit/',
  },
];

export default function Home(): ReactNode {
  const {siteConfig} = useDocusaurusContext();
  return (
    <Layout
      title={siteConfig.title}
      description="Result, Maybe, and railway-oriented error handling for .NET. Zero-allocation core types, rich failure model, full async support, and Native-AOT readiness.">
      <HomepageHeader />
      <main>
        <HomepageFeatures />
        <section className={styles.nextSteps}>
          <div className="container">
            <Heading as="h2" className="text--center">Explore the docs</Heading>
            <div className={styles.nextStepsGrid}>
              {NEXT_STEPS.map((item) => (
                <Link key={item.to} to={item.to} style={{textDecoration: 'none'}}>
                  <div
                    style={{
                      background: 'var(--functional-card-bg)',
                      border: '1px solid var(--functional-card-border)',
                      borderRadius: '10px',
                      padding: '1.25rem',
                      height: '100%',
                      transition: 'box-shadow 0.2s, transform 0.2s',
                    }}
                    onMouseEnter={e => {
                      (e.currentTarget as HTMLDivElement).style.transform = 'translateY(-2px)';
                      (e.currentTarget as HTMLDivElement).style.boxShadow = 'var(--functional-card-shadow-hover)';
                    }}
                    onMouseLeave={e => {
                      (e.currentTarget as HTMLDivElement).style.transform = '';
                      (e.currentTarget as HTMLDivElement).style.boxShadow = '';
                    }}>
                    <div style={{fontSize: '1.75rem', marginBottom: '0.5rem'}}>{item.emoji}</div>
                    <Heading as="h3" style={{fontSize: '1rem', marginBottom: '0.4rem'}}>
                      {item.title}
                    </Heading>
                    <p style={{fontSize: '0.875rem', margin: 0, opacity: 0.75}}>{item.description}</p>
                  </div>
                </Link>
              ))}
            </div>
          </div>
        </section>
      </main>
    </Layout>
  );
}
