import { useState } from "react";
import type { FormEvent } from "react";

export function ContactPage() {
  const [sent, setSent] = useState(false);
  const [form, setForm] = useState({
    name: "",
    email: "",
    topic: "",
    message: "",
  });

  function updateField<K extends keyof typeof form>(key: K, value: string) {
    setForm((state) => ({ ...state, [key]: value }));
  }

  function onSubmit(event: FormEvent) {
    event.preventDefault();
    setSent(true);
    setForm({ name: "", email: "", topic: "", message: "" });
  }

  return (
    <section className="dashboard">
      <div className="hero-panel">
        <h1>Contact Us</h1>
        <p>
          Have questions, partnership ideas, or feature requests? We would love
          to hear from you.
        </p>
      </div>

      {sent ? (
        <div className="notice success">
          Thanks! We got your message and will get back to you soon.
        </div>
      ) : null}

      <div className="info-grid">
        <form className="hero-panel form-stack" onSubmit={onSubmit}>
          <h2>Send a Message</h2>
          <input
            className="input"
            value={form.name}
            onChange={(event) => updateField("name", event.target.value)}
            placeholder="Your name"
            required
          />
          <input
            className="input"
            type="email"
            value={form.email}
            onChange={(event) => updateField("email", event.target.value)}
            placeholder="Your email"
            required
          />
          <input
            className="input"
            value={form.topic}
            onChange={(event) => updateField("topic", event.target.value)}
            placeholder="Topic"
            required
          />
          <textarea
            className="input"
            rows={5}
            value={form.message}
            onChange={(event) => updateField("message", event.target.value)}
            placeholder="How can we help?"
            required
          />
          <button className="btn-primary" type="submit">
            Send Message
          </button>
        </form>

        <article className="hero-panel">
          <h2>Support Details</h2>
          <p className="muted">Email: support@mywishlist.app</p>
          <p className="muted">Partnerships: partners@mywishlist.app</p>
          <p className="muted">Hours: Sun-Thu, 09:00-18:00 (UTC)</p>
          <p className="muted">
            You can also reach us from the app using Thank You and feedback
            flows.
          </p>
        </article>
      </div>
    </section>
  );
}
