import { Link, Navigate, Route, Routes } from "react-router-dom";
import { useEffect, useState } from "react";
import { LoginPage } from "./pages/LoginPage";
import { RegisterPage } from "./pages/RegisterPage";
import { DashboardPage } from "./pages/DashboardPage";
import { WishlistPage } from "./pages/WishlistPage";
import { PublicWishlistPage } from "./pages/PublicWishlistPage";
import { DiscoverPage } from "./pages/DiscoverPage";
import { api } from "./api";
import type { AuthUser } from "./types";

type Notice = { kind: "success" | "error"; message: string } | null;

type AuthContextValue = {
  user: AuthUser | null;
  refreshAuth: () => Promise<void>;
  setUser: (value: AuthUser | null) => void;
  notice: Notice;
  setNotice: (value: Notice) => void;
};

export function App() {
  const [user, setUser] = useState<AuthUser | null>(null);
  const [authReady, setAuthReady] = useState(false);
  const [notice, setNotice] = useState<Notice>(null);

  const refreshAuth = async () => {
    try {
      const profile = await api.me();
      setUser(profile);
    } catch {
      setUser(null);
    } finally {
      setAuthReady(true);
    }
  };

  useEffect(() => {
    void refreshAuth();
  }, []);

  if (!authReady) {
    return <div className="center-screen">Loading your wishlist universe...</div>;
  }

  const context: AuthContextValue = {
    user,
    setUser,
    refreshAuth,
    notice,
    setNotice,
  };

  return (
    <div className="app-shell">
      <header className="topbar">
        <Link to={user ? "/" : "/login"} className="brand">
          MyWishList
        </Link>
        <div className="topbar-right">
          <Link className="ghost-btn link-btn" to="/discover">
            Discover
          </Link>
          {user ? (
            <button
              className="ghost-btn"
              onClick={async () => {
                await api.logout();
                setUser(null);
              }}
            >
              Logout
            </button>
          ) : (
            <Link className="ghost-btn link-btn" to="/login">
              Login
            </Link>
          )}
        </div>
      </header>

      {notice ? <div className={`notice ${notice.kind}`}>{notice.message}</div> : null}

      <main className="content">
        <Routes>
          <Route
            path="/login"
            element={
              user ? <Navigate to="/" replace /> : <LoginPage context={context} />
            }
          />
          <Route
            path="/register"
            element={
              user ? <Navigate to="/" replace /> : <RegisterPage context={context} />
            }
          />
          <Route
            path="/"
            element={
              user ? <DashboardPage context={context} /> : <Navigate to="/login" replace />
            }
          />
          <Route path="/discover" element={<DiscoverPage />} />
          <Route
            path="/wishlists/:id"
            element={
              user ? <WishlistPage context={context} /> : <Navigate to="/login" replace />
            }
          />
          <Route path="/shared/:shareToken" element={<PublicWishlistPage />} />
        </Routes>
      </main>
    </div>
  );
}

export type { AuthContextValue };
