export function AboutPage() {
  return (
    <section className="dashboard">
      <div className="hero-panel photo-hero">
        <div>
          <h1>About MyWishList</h1>
          <p>
            We help families, couples, and communities organize meaningful gift
            moments in one elegant place.
          </p>
        </div>
      </div>

      <div className="info-grid">
        <article className="hero-panel">
          <h2>Our Mission</h2>
          <p>
            Make gifting thoughtful and stress-free by giving everyone a single
            home for registries, wishlists, and event goals.
          </p>
        </article>
        <article className="hero-panel">
          <h2>What We Support</h2>
          <p>
            Wedding, baby, birthday, housewarming, nonprofit, and personal
            milestones with public sharing and reservation tools.
          </p>
        </article>
      </div>

      <div className="photo-strip">
        <div className="photo-card photo-one" />
        <div className="photo-card photo-two" />
        <div className="photo-card photo-three" />
      </div>

      <article className="hero-panel">
        <h2>Why People Love It</h2>
        <p>
          One link to share, clean item tracking, gift status updates, cash
          fund support, and polished mobile-friendly design.
        </p>
      </article>
    </section>
  );
}
