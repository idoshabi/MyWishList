import type {
  AuthUser,
  CashContribution,
  WishlistDetails,
  WishlistSummary,
} from "./types";

const jsonHeaders = { "Content-Type": "application/json" };

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const response = await fetch(path, {
    credentials: "include",
    ...init,
  });

  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || `Request failed with status ${response.status}`);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return (await response.json()) as T;
}

export const api = {
  me: () => request<AuthUser>("/api/auth/me"),

  login: (payload: { usernameOrEmail: string; password: string }) =>
    request<AuthUser>("/api/auth/login", {
      method: "POST",
      headers: jsonHeaders,
      body: JSON.stringify(payload),
    }),

  register: (payload: {
    username: string;
    email: string;
    firstName: string;
    lastName: string;
    password: string;
    dateOfBirth: string;
  }) =>
    request<AuthUser>("/api/auth/register", {
      method: "POST",
      headers: jsonHeaders,
      body: JSON.stringify(payload),
    }),

  logout: () =>
    request<void>("/api/auth/logout", {
      method: "POST",
    }),

  getWishlists: () => request<WishlistSummary[]>("/api/wishlists"),

  createWishlist: (payload: {
    name: string;
    registryType: string;
    visibility: "Public" | "Private";
    cashFundGoal?: number;
    description?: string;
    eventDate?: string;
  }) =>
    request<WishlistDetails>("/api/wishlists", {
      method: "POST",
      headers: jsonHeaders,
      body: JSON.stringify(payload),
    }),

  getWishlist: (id: number) => request<WishlistDetails>(`/api/wishlists/${id}`),

  addItem: (
    wishlistId: number,
    payload: {
      productName: string;
      link?: string;
      merchant?: string;
      type?: string;
    },
  ) =>
    request(`/api/wishlists/${wishlistId}/items`, {
      method: "POST",
      headers: jsonHeaders,
      body: JSON.stringify(payload),
    }),

  reserveItem: (wishlistId: number, itemId: number, reservedByName: string) =>
    request<void>(`/api/wishlists/${wishlistId}/items/${itemId}/reserve`, {
      method: "POST",
      headers: jsonHeaders,
      body: JSON.stringify({ reservedByName }),
    }),

  unreserveItem: (wishlistId: number, itemId: number) =>
    request<void>(`/api/wishlists/${wishlistId}/items/${itemId}/reserve`, {
      method: "DELETE",
    }),

  markPurchased: (wishlistId: number, itemId: number, purchasedByName: string) =>
    request<void>(`/api/wishlists/${wishlistId}/items/${itemId}/purchase`, {
      method: "POST",
      headers: jsonHeaders,
      body: JSON.stringify({ purchasedByName }),
    }),

  markReceived: (wishlistId: number, itemId: number) =>
    request<void>(`/api/wishlists/${wishlistId}/items/${itemId}/received`, {
      method: "POST",
    }),

  getPublicWishlist: (shareToken: string) =>
    request<WishlistDetails>(`/api/public/wishlists/${shareToken}`),

  discoverPublic: (query?: string, type?: string) =>
    request<WishlistSummary[]>(
      `/api/public/discover?query=${encodeURIComponent(query ?? "")}&type=${encodeURIComponent(type ?? "")}`,
    ),

  updateWishlistSettings: (
    wishlistId: number,
    payload: {
      name: string;
      registryType: string;
      visibility: "Public" | "Private";
      cashFundGoal?: number;
      description?: string;
      eventDate?: string;
    },
  ) =>
    request<void>(`/api/wishlists/${wishlistId}/settings`, {
      method: "PUT",
      headers: jsonHeaders,
      body: JSON.stringify(payload),
    }),

  contributeCash: (
    wishlistId: number,
    payload: {
      provider: "Stripe" | "PayPal";
      amount: number;
      contributorName: string;
      contributorEmail?: string;
      message?: string;
    },
  ) =>
    request<CashContribution>(`/api/wishlists/${wishlistId}/contributions`, {
      method: "POST",
      headers: jsonHeaders,
      body: JSON.stringify(payload),
    }),

  getContributions: (wishlistId: number) =>
    request<CashContribution[]>(`/api/wishlists/${wishlistId}/contributions`),

  previewImport: (urls: string[]) =>
    request<{ url: string; productName: string; merchant?: string | null }[]>(
      "/api/platform/import-preview",
      {
        method: "POST",
        headers: jsonHeaders,
        body: JSON.stringify({ urls }),
      },
    ),

  sendThankYou: (payload: {
    recipientName: string;
    recipientEmail: string;
    message?: string;
  }) =>
    request<void>("/api/platform/thank-you", {
      method: "POST",
      headers: jsonHeaders,
      body: JSON.stringify(payload),
    }),

  adminMetrics: (apiKey: string) =>
    request<{ users: number; wishlists: number; items: number; contributions: number }>(
      "/api/admin/metrics",
      {
        headers: { "X-Admin-Key": apiKey },
      },
    ),
};
