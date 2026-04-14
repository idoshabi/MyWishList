import { useEffect, useState } from "react";
import type { FormEvent } from "react";
import { Link } from "react-router-dom";
import { api } from "../api";
import type { AuthContextValue } from "../App";
import type { WishlistSummary } from "../types";

export function DashboardPage({ context }: { context: AuthContextValue }) {
  const [loading, setLoading] = useState(true);
  const [wishlists, setWishlists] = useState<WishlistSummary[]>([]);
  const [name, setName] = useState("");
  const [busy, setBusy] = useState(false);

  async function loadWishlists() {
    setLoading(true);
    try {
      setWishlists(await api.getWishlists());
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    void loadWishlists();
  }, []);

  async function onCreate(event: FormEvent) {
    event.preventDefault();
    if (!name.trim()) return;
    setBusy(true);
    context.setNotice(null);
    try {
      await api.createWishlist({ name });
      setName("");
      context.setNotice({ kind: "success", message: "Wishlist created successfully." });
      await loadWishlists();
    } catch {
      context.setNotice({ kind: "error", message: "Could not create wishlist." });
    } finally {
      setBusy(false);
    }
  }

  return (
    <section className="dashboard">
      <div className="hero-panel">
        <h1>Hey {context.user?.firstName}, what are you manifesting today?</h1>
        <p>
          Keep all your gift ideas, dream gadgets, and milestones in one premium
          looking space.
        </p>
      </div>

      <form className="create-row" onSubmit={onCreate}>
        <input
          className="input"
          value={name}
          onChange={(event) => setName(event.target.value)}
          placeholder="Create a new wishlist (Birthday 2026...)"
          required
        />
        <button className="btn-primary" type="submit" disabled={busy}>
          {busy ? "Creating..." : "Create"}
        </button>
      </form>

      {loading ? <p className="muted">Loading wishlists...</p> : null}

      {!loading && wishlists.length === 0 ? (
        <div className="empty-card">No wishlists yet. Create your first one above.</div>
      ) : null}

      <div className="wishlist-grid">
        {wishlists.map((wishlist) => (
          <article key={wishlist.id} className="wishlist-card">
            <h3>{wishlist.name}</h3>
            <p>{wishlist.itemCount} items</p>
            <Link to={`/wishlists/${wishlist.id}`} className="link-chip">
              Open Wishlist
            </Link>
          </article>
        ))}
      </div>
    </section>
  );
}
