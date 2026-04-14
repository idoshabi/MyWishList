export type AuthUser = {
  id: number;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
};

export type WishlistSummary = {
  id: number;
  name: string;
  itemCount: number;
};

export type WishlistItem = {
  id: number;
  productName: string;
  link?: string | null;
  merchant?: string | null;
  type?: string | null;
};

export type WishlistDetails = {
  id: number;
  name: string;
  items: WishlistItem[];
};
