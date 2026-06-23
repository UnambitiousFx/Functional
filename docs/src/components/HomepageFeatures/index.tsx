import type {CSSProperties, ReactNode} from 'react';
import Heading from '@theme/Heading';
import styles from './styles.module.css';

type FeatureItem = {
  icon: string;
  accent: string;
  title: string;
  description: ReactNode;
};

const FeatureList: FeatureItem[] = [
  {
    icon: '🚂',
    accent: '#4f46e5',
    title: 'Result & Railway',
    description: (
      <>
        Model success and failure as one type. Chain <code>Bind</code>,{' '}
        <code>Map</code>, and <code>Ensure</code> — the first failure
        short-circuits the rest. Errors are values, not exceptions.
      </>
    ),
  },
  {
    icon: '🎁',
    accent: '#7c3aed',
    title: 'Maybe / Optionals',
    description: (
      <>
        Represent "a value or nothing" without <code>null</code>. Compose with{' '}
        <code>Map</code>, <code>Filter</code>, and <code>OrElse</code>, then
        bridge to <code>Result</code> when you need a reason.
      </>
    ),
  },
  {
    icon: '🏷️',
    accent: '#d97706',
    title: 'Rich Failures',
    description: (
      <>
        Typed failures — validation, not-found, conflict, unauthorized — each
        with a machine-readable code and attachable <code>Metadata</code>.
        Aggregate many into one.
      </>
    ),
  },
  {
    icon: '⚡',
    accent: '#0d9488',
    title: 'Zero-Allocation & AOT',
    description: (
      <>
        Core types are <code>readonly struct</code>s with{' '}
        <code>ValueTask</code>-based async. Minimal heap traffic in hot paths
        and fully Native-AOT compatible.
      </>
    ),
  },
];

function Feature({icon, accent, title, description}: FeatureItem) {
  return (
    <div
      className={styles.featureCard}
      style={{'--card-accent': accent} as CSSProperties}>
      <div className={styles.featureIconWrap}>
        <span className={styles.featureIcon}>{icon}</span>
      </div>
      <Heading as="h3" className={styles.featureTitle}>{title}</Heading>
      <p className={styles.featureDescription}>{description}</p>
    </div>
  );
}

export default function HomepageFeatures(): ReactNode {
  return (
    <section className={styles.features}>
      <div className="container">
        <Heading as="h2" className={styles.sectionHeading}>Why Functional?</Heading>
        <div className={styles.featuresGrid}>
          {FeatureList.map((props, idx) => (
            <Feature key={idx} {...props} />
          ))}
        </div>
      </div>
    </section>
  );
}
