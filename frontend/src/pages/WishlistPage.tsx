import { useEffect, useState } from "react";
import type { FormEvent } from "react";
import { Link, useParams } from "react-router-dom";
import { api } from "../api";
import type { AuthContextValue } from "../App";
import type { CashContribution, WishlistDetails } from "../types";

export function WishlistPage({ context }: { context: AuthContextValue }) {
  const params = useParams();
  const wishlistId = Number(params.id);
  const [wishlist, setWishlist] = useState<WishlistDetails | null>(null);
  const [contributions, setContributions] = useState<CashContribution[]>([]);
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [form, setForm] = useState({
    productName: "",
    link: "",
    merchant: "",
    type: "",
  });
  const [reserveBy, setReserveBy] = useState("");
  const [purchaseBy, setPurchaseBy] = useState("");
  const [settingsBusy, setSettingsBusy] = useState(false);
  const [settingsForm, setSettingsForm] = useState({
    name: "",
    registryType: "General",
    visibility: "Private" as "Public" | "Private",
    cashFundGoal: "",
    description: "",
    eventDate: "",
  });
  const [contributionForm, setContributionForm] = useState({
    provider: "Stripe" as "Stripe" | "PayPal",
    amount: "",
    contributorName: "",
    contributorEmail: "",
    message: "",
  });
  const [importUrls, setImportUrls] = useState("");
  const [importPreview, setImportPreview] = useState<
    { url: string; productName: string; merchant?: string | null }[]
  >([]);

  async function loadWishlist() {
    if (!Number.isFinite(wishlistId)) {
      setLoading(false);
      return;
    }

    setLoading(true);
    try {
      setWishlist(await api.getWishlist(wishlistId));
      setContributions(await api.getContributions(wishlistId));
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

  useEffect(() => {
    if (!wishlist) return;
    setSettingsForm({
      name: wishlist.name,
      registryType: wishlist.registryType,
      visibility: wishlist.visibility,
      cashFundGoal: wishlist.cashFundGoal ? String(wishlist.cashFundGoal) : "",
      description: wishlist.description ?? "",
      eventDate: wishlist.eventDate ?? "",
    });
  }, [wishlist]);

  async function onSubmit(event: FormEvent) {
    event.preventDefault();
    if (!wishlistId || !form.productName.trim()) return;

    setBusy(true);
    context.setNotice(null);
    try {
      await api.addItem(wishlistId, form);
      setForm({ productName: "", link: "", merchant: "", type: "" });
      context.setNotice({ kind: "success", message: "Item queued. It will appear shortly." });
      await loadWishlist();
    } catch {
      context.setNotice({ kind: "error", message: "Could not add item." });
    } finally {
      setBusy(false);
    }
  }

  async function onSaveSettings(event: FormEvent) {
    event.preventDefault();
    if (!wishlistId) return;
    setSettingsBusy(true);
    try {
      await api.updateWishlistSettings(wishlistId, {
        name: settingsForm.name,
        registryType: settingsForm.registryType,
        visibility: settingsForm.visibility,
        cashFundGoal: settingsForm.cashFundGoal ? Number(settingsForm.cashFundGoal) : undefined,
        description: settingsForm.description,
        eventDate: settingsForm.eventDate || undefined,
      });
      context.setNotice({ kind: "success", message: "Wishlist settings saved." });
      await loadWishlist();
    } catch {
      context.setNotice({ kind: "error", message: "Could not save wishlist settings." });
    } finally {
      setSettingsBusy(false);
    }
  }

  async function toggleReserve(itemId: number, shouldReserve: boolean) {
    if (!wishlistId) return;

    try {
      if (shouldReserve) {
        if (!reserveBy.trim()) {
          context.setNotice({ kind: "error", message: "Enter your name before reserving." });
          return;
        }

        await api.reserveItem(wishlistId, itemId, reserveBy);
      } else {
        await api.unreserveItem(wishlistId, itemId);
      }

      await loadWishlist();
    } catch {
      context.setNotice({ kind: "error", message: "Could not update reservation." });
    }
  }

  async function togglePurchased(itemId: number, shouldPurchase: boolean) {
    if (!wishlistId) return;
    try {
      if (shouldPurchase) {
        if (!purchaseBy.trim()) {
          context.setNotice({ kind: "error", message: "Enter purchaser name first." });
          return;
        }
        await api.markPurchased(wishlistId, itemId, purchaseBy);
      } else {
        context.setNotice({ kind: "error", message: "Purchased items cannot be unmarked in this MVP." });
        return;
      }

      await loadWishlist();
    } catch {
      context.setNotice({ kind: "error", message: "Could not update purchase state." });
    }
  }

  async function markReceived(itemId: number) {
    if (!wishlistId) return;
    try {
      await api.markReceived(wishlistId, itemId);
      await loadWishlist();
    } catch {
      context.setNotice({ kind: "error", message: "Could not mark item as received." });
    }
  }

  async function submitContribution(event: FormEvent) {
    event.preventDefault();
    if (!wishlistId) return;
    try {
      await api.contributeCash(wishlistId, {
        provider: contributionForm.provider,
        amount: Number(contributionForm.amount),
        contributorName: contributionForm.contributorName,
        contributorEmail: contributionForm.contributorEmail || undefined,
        message: contributionForm.message || undefined,
      });

      setContributionForm((state) => ({ ...state, amount: "", message: "" }));
      await loadWishlist();
      context.setNotice({ kind: "success", message: "Contribution recorded." });
    } catch {
      context.setNotice({ kind: "error", message: "Could not process contribution." });
    }
  }

  async function runImportPreview() {
    const urls = importUrls
      .split("\n")
      .map((value) => value.trim())
      .filter(Boolean);

    if (!urls.length) {
      setImportPreview([]);
      return;
    }

    try {
      setImportPreview(await api.previewImport(urls));
    } catch {
      context.setNotice({ kind: "error", message: "Import preview failed." });
    }
  }

  async function sendThankYou(contributor: CashContribution) {
    try {
      if (!contributor.contributorEmail) {
        context.setNotice({ kind: "error", message: "Contributor email missing." });
        return;
      }

      await api.sendThankYou({
        recipientName: contributor.contributorName,
        recipientEmail: contributor.contributorEmail,
        message: `Thank you ${contributor.contributorName} for contributing $${contributor.amount.toFixed(2)} to ${wishlist?.name}!`,
      });
      context.setNotice({ kind: "success", message: "Thank-you email sent." });
    } catch {
      context.setNotice({ kind: "error", message: "Could not send thank-you email." });
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
        <div>
          <h1>{wishlist.name}</h1>
          <p className="muted">
            {wishlist.registryType} - {wishlist.visibility}
          </p>
          {wishlist.cashFundGoal ? (
            <p className="muted">
              Cash fund: ${wishlist.cashFundRaised.toFixed(0)} / ${wishlist.cashFundGoal.toFixed(0)}
            </p>
          ) : null}
        </div>
        <Link className="ghost-btn link-btn" to="/">
          Back to dashboard
        </Link>
      </div>

      <form className="item-form" onSubmit={onSaveSettings}>
        <input
          className="input"
          value={settingsForm.name}
          onChange={(event) => setSettingsForm({ ...settingsForm, name: event.target.value })}
          placeholder="Wishlist name"
          required
        />
        <select
          className="input"
          value={settingsForm.registryType}
          onChange={(event) =>
            setSettingsForm({ ...settingsForm, registryType: event.target.value })
          }
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
          value={settingsForm.visibility}
          onChange={(event) =>
            setSettingsForm({
              ...settingsForm,
              visibility: event.target.value as "Public" | "Private",
            })
          }
        >
          <option value="Private">Private</option>
          <option value="Public">Public</option>
        </select>
        <input
          className="input"
          type="number"
          min="0"
          step="0.01"
          value={settingsForm.cashFundGoal}
          onChange={(event) =>
            setSettingsForm({ ...settingsForm, cashFundGoal: event.target.value })
          }
          placeholder="Cash goal"
        />
        <button className="btn-primary" disabled={settingsBusy} type="submit">
          {settingsBusy ? "Saving..." : "Save Settings"}
        </button>
      </form>

      <div className="create-row">
        <input
          className="input"
          value={settingsForm.description}
          onChange={(event) =>
            setSettingsForm({ ...settingsForm, description: event.target.value })
          }
          placeholder="Description"
        />
        <input
          className="input"
          type="date"
          value={settingsForm.eventDate}
          onChange={(event) =>
            setSettingsForm({ ...settingsForm, eventDate: event.target.value })
          }
        />
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

      <div className="create-row">
        <input
          className="input"
          value={reserveBy}
          onChange={(event) => setReserveBy(event.target.value)}
          placeholder="Your name for reserve actions"
        />
      </div>

      <div className="create-row">
        <input
          className="input"
          value={purchaseBy}
          onChange={(event) => setPurchaseBy(event.target.value)}
          placeholder="Purchaser name for purchase actions"
        />
      </div>

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
                <th>Status</th>
                <th>Action</th>
                <th>Purchase</th>
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
                  <td>
                    <button
                      className="ghost-btn"
                      type="button"
                      onClick={() => void togglePurchased(item.id, !item.isPurchased)}
                    >
                      {item.isPurchased
                        ? item.isReceived
                          ? "Received"
                          : "Purchased"
                        : "Mark Purchased"}
                    </button>
                    {!item.isReceived && item.isPurchased ? (
                      <button
                        className="ghost-btn"
                        type="button"
                        onClick={() => void markReceived(item.id)}
                      >
                        Mark Received
                      </button>
                    ) : null}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      <form className="item-form" onSubmit={submitContribution}>
        <select
          className="input"
          value={contributionForm.provider}
          onChange={(event) =>
            setContributionForm({
              ...contributionForm,
              provider: event.target.value as "Stripe" | "PayPal",
            })
          }
        >
          <option value="Stripe">Stripe</option>
          <option value="PayPal">PayPal</option>
        </select>
        <input
          className="input"
          type="number"
          min="1"
          step="0.01"
          value={contributionForm.amount}
          onChange={(event) =>
            setContributionForm({ ...contributionForm, amount: event.target.value })
          }
          placeholder="Contribution amount"
          required
        />
        <input
          className="input"
          value={contributionForm.contributorName}
          onChange={(event) =>
            setContributionForm({ ...contributionForm, contributorName: event.target.value })
          }
          placeholder="Contributor name"
          required
        />
        <input
          className="input"
          type="email"
          value={contributionForm.contributorEmail}
          onChange={(event) =>
            setContributionForm({ ...contributionForm, contributorEmail: event.target.value })
          }
          placeholder="Contributor email"
        />
        <button className="btn-primary" type="submit">
          Contribute Cash
        </button>
      </form>

      {contributions.length ? (
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Contributor</th>
                <th>Amount</th>
                <th>Provider</th>
                <th>Status</th>
                <th>Thank you</th>
              </tr>
            </thead>
            <tbody>
              {contributions.map((entry) => (
                <tr key={entry.id}>
                  <td>{entry.contributorName}</td>
                  <td>${entry.amount.toFixed(2)}</td>
                  <td>{entry.provider}</td>
                  <td>{entry.status}</td>
                  <td>
                    <button
                      className="ghost-btn"
                      type="button"
                      onClick={() => void sendThankYou(entry)}
                    >
                      Send Thank You
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      ) : null}

      <div className="table-wrap">
        <div className="dashboard">
          <h3>External registry import preview</h3>
          <textarea
            className="input"
            rows={4}
            value={importUrls}
            onChange={(event) => setImportUrls(event.target.value)}
            placeholder="Paste one product URL per line"
          />
          <button className="ghost-btn" type="button" onClick={() => void runImportPreview()}>
            Preview Imported Items
          </button>
          {importPreview.map((value) => (
            <p key={value.url} className="muted small">
              {value.productName} ({value.merchant ?? "Unknown"}) - {value.url}
            </p>
          ))}
        </div>
      </div>
    </section>
  );
}
