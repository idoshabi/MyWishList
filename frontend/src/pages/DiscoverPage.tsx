import { useState } from "react";
import type { FormEvent } from "react";
import { Link } from "react-router-dom";
import { api } from "../api";
import type { WishlistSummary } from "../types";

export function DiscoverPage() {
  const [query, setQuery] = useState("");
  const [type, setType] = useState("");
  const [results, setResults] = useState<WishlistSummary[]>([]);

  async function onSearch(event: FormEvent) {
    event.preventDefault();
    setResults(await api.discoverPublic(query, type));
  }

  return (
    <section className="dashboard">
      <div className="hero-panel">
        <h1>Discover Public Registries</h1>
        <p>Find wedding, baby, birthday, and nonprofit registries shared publicly.</p>
      </div>

      <form className="create-row" onSubmit={onSearch}>
        <input
          className="input"
          placeholder="Search by name/description"
          value={query}
          onChange={(event) => setQuery(event.target.value)}
        />
        <select
          className="input"
          value={type}
          onChange={(event) => setType(event.target.value)}
        >
          <option value="">All types</option>
          <option value="Wedding">Wedding</option>
          <option value="Baby">Baby</option>
          <option value="Birthday">Birthday</option>
          <option value="Housewarming">Housewarming</option>
          <option value="Nonprofit">Nonprofit</option>
          <option value="General">General</option>
        </select>
        <button className="btn-primary" type="submit">
          Search
        </button>
      </form>

      <div className="wishlist-grid">
        {results.map((wishlist) => (
          <article className="wishlist-card" key={wishlist.id}>
            <h3>{wishlist.name}</h3>
            <p className="muted small">{wishlist.registryType}</p>
            <p className="muted small">{wishlist.description ?? "No description"}</p>
            <Link className="link-chip" to={`/shared/${wishlist.shareToken}`}>
              View Registry
            </Link>
          </article>
        ))}
      </div>
    </section>
  );
}
