import Link from '@docusaurus/Link';

export default function HomepageComplements() {
  return (
    <section style={{ padding: '4rem 0', textAlign: 'center' }}>
        <div className="container">
          <h2>Why Vectantic?</h2>
          <p style={{ maxWidth: 700, margin: '1rem auto' }}>
            Most embedding solutions rely on external APIs, cloud inference, 
            or heavyweight Python stacks.
            Vectantic provides a native .NET-first experience for local semantic embeddings 
            and vector search using ONNX Runtime — optimized for modern backend applications.
          </p>

          <div style={{ marginTop: '2rem' }}>
            <Link className="button button--primary button--lg" href="/docs/intro">
              Get Started
            </Link>
            <a className="button button--secondary button--lg"
              href="https://github.com/JSebas-11/Vectantic"
              target='_blank'
              style={{ marginLeft: '1rem' }}>
              View on GitHub
            </a>
          </div>
        </div>
      </section>
  );
}