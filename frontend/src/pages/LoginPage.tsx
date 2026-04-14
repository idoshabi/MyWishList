import { useState } from "react";
import type { FormEvent } from "react";
import { Link, useNavigate } from "react-router-dom";
import { api } from "../api";
import type { AuthContextValue } from "../App";

export function LoginPage({ context }: { context: AuthContextValue }) {
  const navigate = useNavigate();
  const [usernameOrEmail, setUsernameOrEmail] = useState("");
  const [password, setPassword] = useState("");
  const [busy, setBusy] = useState(false);

  async function onSubmit(event: FormEvent) {
    event.preventDefault();
    setBusy(true);
    context.setNotice(null);
    try {
      const user = await api.login({ usernameOrEmail, password });
      context.setUser(user);
      context.setNotice({ kind: "success", message: `Welcome back, ${user.firstName}!` });
      navigate("/");
    } catch {
      context.setNotice({ kind: "error", message: "Invalid username/email or password." });
    } finally {
      setBusy(false);
    }
  }

  return (
    <section className="auth-card">
      <h1>Welcome Back</h1>
      <p>Sign in to continue building your dream list.</p>
      <form onSubmit={onSubmit} className="form-stack">
        <input
          className="input"
          value={usernameOrEmail}
          onChange={(event) => setUsernameOrEmail(event.target.value)}
          placeholder="Username or email"
          required
        />
        <input
          className="input"
          value={password}
          onChange={(event) => setPassword(event.target.value)}
          type="password"
          placeholder="Password"
          required
        />
        <button className="btn-primary" disabled={busy} type="submit">
          {busy ? "Signing in..." : "Sign In"}
        </button>
      </form>
      <small className="muted">
        New here? <Link to="/register">Create your account</Link>
      </small>
    </section>
  );
}
