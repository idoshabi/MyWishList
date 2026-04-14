import { useState } from "react";
import type { FormEvent } from "react";
import { Link, useNavigate } from "react-router-dom";
import { api } from "../api";
import type { AuthContextValue } from "../App";

export function RegisterPage({ context }: { context: AuthContextValue }) {
  const navigate = useNavigate();
  const [busy, setBusy] = useState(false);
  const [form, setForm] = useState({
    username: "",
    email: "",
    firstName: "",
    lastName: "",
    password: "",
    dateOfBirth: "",
  });

  function setField(field: keyof typeof form, value: string) {
    setForm((state) => ({ ...state, [field]: value }));
  }

  async function onSubmit(event: FormEvent) {
    event.preventDefault();
    setBusy(true);
    context.setNotice(null);
    try {
      const user = await api.register(form);
      context.setUser(user);
      context.setNotice({
        kind: "success",
        message: `Account ready. Welcome, ${user.firstName}!`,
      });
      navigate("/");
    } catch {
      context.setNotice({
        kind: "error",
        message: "Could not register. Check your details and try again.",
      });
    } finally {
      setBusy(false);
    }
  }

  return (
    <section className="auth-card wide">
      <h1>Create Account</h1>
      <p>Build beautiful wishlists for every milestone.</p>
      <form onSubmit={onSubmit} className="form-grid">
        <input
          className="input"
          value={form.firstName}
          onChange={(event) => setField("firstName", event.target.value)}
          placeholder="First name"
          required
        />
        <input
          className="input"
          value={form.lastName}
          onChange={(event) => setField("lastName", event.target.value)}
          placeholder="Last name"
          required
        />
        <input
          className="input"
          value={form.username}
          onChange={(event) => setField("username", event.target.value)}
          placeholder="Username"
          required
        />
        <input
          className="input"
          value={form.email}
          onChange={(event) => setField("email", event.target.value)}
          type="email"
          placeholder="Email"
          required
        />
        <input
          className="input"
          value={form.dateOfBirth}
          onChange={(event) => setField("dateOfBirth", event.target.value)}
          type="date"
          required
        />
        <input
          className="input"
          value={form.password}
          onChange={(event) => setField("password", event.target.value)}
          type="password"
          placeholder="Password"
          required
        />
        <button className="btn-primary full-span" disabled={busy} type="submit">
          {busy ? "Creating..." : "Create Account"}
        </button>
      </form>
      <small className="muted">
        Already have an account? <Link to="/login">Sign in</Link>
      </small>
    </section>
  );
}
