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
  const [registryType, setRegistryType] = useState("General");
  const [visibility, setVisibility] = useState<"Public" | "Private">("Private");
  const [cashFundGoal, setCashFundGoal] = useState("");
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
      await api.createWishlist({
        name,
        registryType,
        visibility,
        cashFundGoal: cashFundGoal ? Number(cashFundGoal) : undefined,
      });
      setName("");
      setRegistryType("General");
      setVisibility("Private");
      setCashFundGoal("");
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

      <div className="photo-strip">
        <div className="photo-card photo-one" />
        <div className="photo-card photo-two" />
        <div className="photo-card photo-three" />
      </div>

      <form className="create-row" onSubmit={onCreate}>
        <input
          className="input"
          value={name}
          onChange={(event) => setName(event.target.value)}
          placeholder="Create a new wishlist (Birthday 2026...)"
          required
        />
        <select
          className="input"
          value={registryType}
          onChange={(event) => setRegistryType(event.target.value)}
        >
          <option value="General">General</option>
          <option value="Wedding">Wedding</option>
          <option value="Baby">Baby</option>
          <option value="Birthday">Birthday</option>
          <option value="Housewarming">Housewarming</option>
          <option value="Nonprofit">Nonprofit</option>
        </select>
        <select
          className="input"
          value={visibility}
          onChange={(event) => setVisibility(event.target.value as "Public" | "Private")}
        >
          <option value="Private">Private</option>
          <option value="Public">Public</option>
        </select>
        <input
          className="input"
          type="number"
          min="0"
          step="0.01"
          value={cashFundGoal}
          onChange={(event) => setCashFundGoal(event.target.value)}
          placeholder="Cash goal (optional)"
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
            <p>{wishlist.registryType} - {wishlist.itemCount} items</p>
            <p className="muted small">{wishlist.visibility} list</p>
            {wishlist.visibility === "Public" ? (
              <p className="muted small">Share: <code>{wishlist.shareToken}</code></p>
            ) : null}
            {wishlist.cashFundGoal ? (
              <p className="muted small">
                Cash fund: ${wishlist.cashFundRaised.toFixed(0)} / ${wishlist.cashFundGoal.toFixed(0)}
              </p>
            ) : null}
            <Link to={`/wishlists/${wishlist.id}`} className="link-chip">
              Open Wishlist
            </Link>
            {wishlist.visibility === "Public" ? (
              <Link to={`/shared/${wishlist.shareToken}`} className="link-chip">
                Open Public View
              </Link>
            ) : null}
          </article>
        ))}
      </div>
    </section>
  );
}
