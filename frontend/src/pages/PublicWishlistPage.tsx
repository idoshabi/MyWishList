import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { api } from "../api";
import type { WishlistDetails } from "../types";

export function PublicWishlistPage() {
  const { shareToken } = useParams();
  const [wishlist, setWishlist] = useState<WishlistDetails | null>(null);
  const [loading, setLoading] = useState(true);
  const [reserveBy, setReserveBy] = useState("");
  const [error, setError] = useState("");

  async function load() {
    if (!shareToken) return;
    setLoading(true);
    setError("");
    try {
      setWishlist(await api.getPublicWishlist(shareToken));
    } catch {
      setWishlist(null);
      setError("This public registry could not be found.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    void load();
  }, [shareToken]);

  async function toggleReserve(itemId: number, shouldReserve: boolean) {
    if (!wishlist) return;
    if (shouldReserve && !reserveBy.trim()) {
      setError("Enter your name before reserving an item.");
      return;
    }

    try {
      if (shouldReserve) {
        await api.reserveItem(wishlist.id, itemId, reserveBy);
      } else {
        await api.unreserveItem(wishlist.id, itemId);
      }

      await load();
    } catch {
      setError("Could not update reservation.");
    }
  }

  if (loading) {
    return <p className="muted">Loading shared registry...</p>;
  }

  if (!wishlist) {
    return (
      <div className="empty-card">
        {error || "Registry not found."} <Link to="/login">Go to app</Link>
      </div>
    );
  }

  return (
    <section className="dashboard">
      <div className="hero-panel">
        <h1>{wishlist.name}</h1>
        <p>
          Public {wishlist.registryType} registry
          {wishlist.cashFundGoal
            ? ` - Cash goal $${wishlist.cashFundRaised.toFixed(0)} / $${wishlist.cashFundGoal.toFixed(0)}`
            : ""}
        </p>
      </div>

      <div className="create-row">
        <input
          className="input"
          value={reserveBy}
          onChange={(event) => setReserveBy(event.target.value)}
          placeholder="Your name for reserve actions"
        />
      </div>

      {error ? <div className="notice error">{error}</div> : null}

      <div className="table-wrap">
        <table>
          <thead>
            <tr>
              <th>Product</th>
              <th>Merchant</th>
              <th>Type</th>
              <th>Link</th>
              <th>Status</th>
              <th>Action</th>
            </tr>
          </thead>
          <tbody>
            {wishlist.items.map((item) => (
              <tr key={item.id}>
                <td>{item.productName}</td>
                <td>{item.merchant || "-"}</td>
                <td>{item.type || "-"}</td>
                <td>
                  {item.link ? (
                    <a href={item.link} target="_blank" rel="noreferrer">
                      Open
                    </a>
                  ) : (
                    "-"
                  )}
                </td>
                <td>
                  {item.isReserved
                    ? `Reserved by ${item.reservedByName ?? "someone"}`
                    : "Available"}
                </td>
                <td>
                  <button
                    className="ghost-btn"
                    onClick={() => void toggleReserve(item.id, !item.isReserved)}
                    type="button"
                  >
                    {item.isReserved ? "Unreserve" : "Reserve"}
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </section>
  );
}
