import type {ReactNode} from 'react';
import clsx from 'clsx';
import Heading from '@theme/Heading';
import styles from './styles.module.css';

type FeatureItem = {
  title: string;
  Svg: React.ComponentType<React.ComponentProps<'svg'>>;
  description: ReactNode;
};

const FeatureList: FeatureItem[] = [
  {
    title: '100% Local Inference',
    Svg: require('@site/static/img/undraw_docusaurus_mountain.svg').default,
    description: (
      <>
        Run embeddings and semantic search directly on the developer machine 
        using ONNX Runtime. No external APIs, no cloud dependency, no token costs.
      </>
    ),
  },
  {
    title: 'Batch Embeddings',
    Svg: require('@site/static/img/undraw_docusaurus_tree.svg').default,
    description: (
      <>
        Process multiple documents in a single ONNX inference pass for 
        significantly better throughput compared to sequential embedding generation.
      </>
    ),
  },
  {
    title: 'Built for Modern .NET',
    Svg: require('@site/static/img/undraw_docusaurus_react.svg').default,
    description: (
      <>
        Native dependency injection support, async-first APIs, 
        ConfigureAwait(false) across the entire library, and targeting net8.0 + net9.0.
      </>
    ),
  },
  {
    title: 'Semantic Search Pipeline',
    Svg: require('@site/static/img/undraw_docusaurus_react.svg').default,
    description: (
      <>
        Generate normalized embeddings optimized for cosine similarity and 
        high-performance vector search operations.
      </>
    ),
  },
  {
    title: 'Modular Architecture',
    Svg: require('@site/static/img/undraw_docusaurus_react.svg').default,
    description: (
      <>
        Separate Core, Semantic, and Math packages let you use only the components 
        your application actually needs.
      </>
    ),
  },
  {
    title: 'Standalone Vector Math',
    Svg: require('@site/static/img/undraw_docusaurus_react.svg').default,
    description: (
      <>
        Use Vectantic.Math independently for cosine similarity, normalization, 
        Top-K retrieval, and vector operations without ONNX dependencies.
      </>
    ),
  },
];

function Feature({title, Svg, description}: FeatureItem) {
  return (
    <div className={clsx('col col--4')}>
      <div className="text--center">
        <Svg className={styles.featureSvg} role="img" />
      </div>
      <div className="text--center padding-horiz--md">
        <Heading as="h3">{title}</Heading>
        <p>{description}</p>
      </div>
    </div>
  );
}

export default function HomepageFeatures(): ReactNode {
  return (
    <section className={styles.features}>
      <div className="container">
        <div className="row">
          {FeatureList.map((props, idx) => (
            <Feature key={idx} {...props} />
          ))}
        </div>
      </div>
    </section>
  );
}
