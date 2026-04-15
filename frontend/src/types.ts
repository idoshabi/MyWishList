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
  registryType: string;
  visibility: "Public" | "Private";
  cashFundGoal?: number | null;
  cashFundRaised: number;
  description?: string | null;
  eventDate?: string | null;
  shareToken: string;
  itemCount: number;
};

export type WishlistItem = {
  id: number;
  productName: string;
  link?: string | null;
  merchant?: string | null;
  type?: string | null;
  isReserved: boolean;
  reservedByName?: string | null;
  reservedAtUtc?: string | null;
  isPurchased: boolean;
  purchasedByName?: string | null;
  purchasedAtUtc?: string | null;
  isReceived: boolean;
  receivedAtUtc?: string | null;
};

export type WishlistDetails = {
  id: number;
  name: string;
  registryType: string;
  visibility: "Public" | "Private";
  cashFundGoal?: number | null;
  cashFundRaised: number;
  description?: string | null;
  eventDate?: string | null;
  shareToken: string;
  items: WishlistItem[];
};

export type CashContribution = {
  id: number;
  provider: string;
  amount: number;
  contributorName: string;
  contributorEmail?: string | null;
  message?: string | null;
  status: string;
  createdAtUtc: string;
};
