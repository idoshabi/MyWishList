import type { AuthUser, WishlistDetails, WishlistSummary } from "./types";

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

  createWishlist: (payload: { name: string }) =>
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
};
