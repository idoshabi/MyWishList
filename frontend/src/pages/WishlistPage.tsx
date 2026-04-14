import { useEffect, useState } from "react";
import type { FormEvent } from "react";
import { Link, useParams } from "react-router-dom";
import { api } from "../api";
import type { AuthContextValue } from "../App";
import type { WishlistDetails } from "../types";

export function WishlistPage({ context }: { context: AuthContextValue }) {
  const params = useParams();
  const wishlistId = Number(params.id);
  const [wishlist, setWishlist] = useState<WishlistDetails | null>(null);
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [form, setForm] = useState({
    productName: "",
    link: "",
    merchant: "",
    type: "",
  });

  async function loadWishlist() {
    if (!Number.isFinite(wishlistId)) {
      setLoading(false);
      return;
    }

    setLoading(true);
    try {
      setWishlist(await api.getWishlist(wishlistId));
    } catch {
      setWishlist(null);
      context.setNotice({ kind: "error", message: "Wishlist not found." });
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    void loadWishlist();
  }, [wishlistId]);

  async function onSubmit(event: FormEvent) {
    event.preventDefault();
    if (!wishlistId || !form.productName.trim()) return;

    setBusy(true);
    context.setNotice(null);
    try {
      await api.addItem(wishlistId, form);
      setForm({ productName: "", link: "", merchant: "", type: "" });
      context.setNotice({ kind: "success", message: "Item added." });
      await loadWishlist();
    } catch {
      context.setNotice({ kind: "error", message: "Could not add item." });
    } finally {
      setBusy(false);
    }
  }

  if (loading) {
    return <p className="muted">Loading wishlist...</p>;
  }

  if (!wishlist) {
    return (
      <div className="empty-card">
        Wishlist not available. <Link to="/">Go back</Link>
      </div>
    );
  }

  return (
    <section className="dashboard">
      <div className="page-heading">
        <h1>{wishlist.name}</h1>
        <Link className="ghost-btn link-btn" to="/">
          Back to dashboard
        </Link>
      </div>

      <form className="item-form" onSubmit={onSubmit}>
        <input
          className="input"
          value={form.productName}
          onChange={(event) => setForm({ ...form, productName: event.target.value })}
          placeholder="Product name"
          required
        />
        <input
          className="input"
          value={form.link}
          onChange={(event) => setForm({ ...form, link: event.target.value })}
          placeholder="Product link (optional)"
        />
        <input
          className="input"
          value={form.merchant}
          onChange={(event) => setForm({ ...form, merchant: event.target.value })}
          placeholder="Merchant (optional)"
        />
        <input
          className="input"
          value={form.type}
          onChange={(event) => setForm({ ...form, type: event.target.value })}
          placeholder="Type (optional)"
        />
        <button className="btn-primary" disabled={busy} type="submit">
          {busy ? "Adding..." : "Add Item"}
        </button>
      </form>

      {wishlist.items.length === 0 ? (
        <div className="empty-card">No items yet - add your first one above.</div>
      ) : (
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Product</th>
                <th>Merchant</th>
                <th>Type</th>
                <th>Link</th>
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
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </section>
  );
}
