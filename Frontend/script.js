/* ===========================================================
   Coastal Fishing Harbour Portal — script.js
   Shared header/footer injection + page interactions.
   =========================================================== */

const NAV_ITEMS = [
  { href: "index.html", label: "Home" },
  { href: "about.html", label: "About" },
  { href: "register.html", label: "Register Boat" },
  { href: "market.html", label: "Fish Market" },
  { href: "dashboard.html", label: "Status" },
  { href: "contact.html", label: "Contact" },
];

function currentPage() {
  const path = window.location.pathname.split("/").pop();
  return path === "" ? "index.html" : path;
}

function renderHeader() {
  const mount = document.getElementById("site-header");
  if (!mount) return;
  const here = currentPage();
  const links = NAV_ITEMS.map(
    (item) =>
      `<li><a href="${item.href}" class="${item.href === here ? "active" : ""}">${item.label}</a></li>`
  ).join("");

  mount.innerHTML = `
    <div class="nav-wrap">
      <a href="index.html" class="brand">
        <svg class="brand-mark" viewBox="0 0 32 32" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M4 22c2.5 2 5 2 7.5 0s5-2 7.5 0 5 2 7.5 0" stroke="#E3B567" stroke-width="2" stroke-linecap="round"/>
          <path d="M16 4v12M16 16l-6 3M16 16l6 3" stroke="#F6F1E4" stroke-width="2" stroke-linecap="round"/>
          <path d="M10 19c0-3 2.5-3 6-3s6 0 6 3" stroke="#F6F1E4" stroke-width="2" stroke-linecap="round"/>
        </svg>
        <span class="brand-text">Coastal Harbour<small>Fishing Port Authority</small></span>
      </a>
      <button class="nav-toggle" aria-label="Toggle menu" aria-expanded="false" id="navToggle">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M3 6h18M3 12h18M3 18h18" stroke-linecap="round"/></svg>
      </button>
      <ul class="nav-links" id="navLinks">${links}</ul>
    </div>
  `;

  const toggle = document.getElementById("navToggle");
  const navLinks = document.getElementById("navLinks");
  toggle.addEventListener("click", () => {
    const isOpen = navLinks.classList.toggle("open");
    toggle.setAttribute("aria-expanded", String(isOpen));
  });
}

function renderFooter() {
  const mount = document.getElementById("site-footer");
  if (!mount) return;
  mount.innerHTML = `
    <div class="container">
      <div class="footer-grid">
        <div>
          <h4>Coastal Fishing Harbour</h4>
          <p>A working harbour on the Coastal Road, serving local fishermen with docking, auction, cold storage and transport since the village's earliest boats came ashore.</p>
        </div>
        <div>
          <h4>Sitemap</h4>
          <ul>
            <li><a href="index.html">Home</a></li>
            <li><a href="about.html">About Harbour</a></li>
            <li><a href="register.html">Boat Registration</a></li>
            <li><a href="market.html">Fish Market</a></li>
            <li><a href="dashboard.html">Harbour Status</a></li>
          </ul>
        </div>
        <div>
          <h4>Office</h4>
          <ul>
            <li>Coastal Road, Harbour Ward</li>
            <li>+91 XXXXX XXXXX</li>
            <li>harbour@example.com</li>
          </ul>
        </div>
      </div>
      <div class="footer-bottom">
        <span>&copy; ${new Date().getFullYear()} Coastal Fishing Harbour Portal</span>
        <span>Built for fishermen, by the harbour office</span>
      </div>
    </div>
  `;
}

/* ---------- Boat Registration form ---------- */
function setupRegistrationForm() {
  const form = document.getElementById("boatForm");
  if (!form) return;
  const confirmBanner = document.getElementById("confirmBanner");
  const logTable = document.getElementById("logTableBody");

  function showError(fieldId, message) {
    const field = document.getElementById(fieldId).closest(".field");
    field.classList.add("invalid");
    field.querySelector(".error-msg").textContent = message;
  }
  function clearError(fieldId) {
    document.getElementById(fieldId).closest(".field").classList.remove("invalid");
  }

  async function renderEntries() {
    if (!logTable) return;

    const response = await fetch("/api/Boats");
    const entries = await response.json();

    if (entries.length === 0) {
        logTable.innerHTML =
            `<tr><td colspan="5">No boats registered yet.</td></tr>`;
        return;
    }

    logTable.innerHTML = entries.map((e, i) => `
        <tr>
            <td>${i + 1}</td>
            <td>${e.boatName}</td>
            <td>${e.ownerName}</td>
            <td>${e.registrationNumber}</td>
            <td>${e.capacity}</td>
        </tr>
    `).join("");
}

renderEntries();

  form.addEventListener("submit", (e) => {
    e.preventDefault();
    let valid = true;
    const boatName = document.getElementById("boatName");
    const owner = document.getElementById("owner");
    const boatNumber = document.getElementById("boatNumber");
    const fishingType = document.getElementById("fishingType");
    const contact = document.getElementById("contact");

    [boatName.id, owner.id, boatNumber.id, fishingType.id, contact.id].forEach(clearError);

    if (!boatName.value.trim()) { showError(boatName.id, "Enter the boat's name."); valid = false; }
    if (!owner.value.trim()) { showError(owner.id, "Enter the owner's name."); valid = false; }
    if (!boatNumber.value.trim()) { showError(boatNumber.id, "Enter the registration number."); valid = false; }
    if (!fishingType.value) { showError(fishingType.id, "Select a fishing type."); valid = false; }
    if (!/^[0-9+\-\s]{8,15}$/.test(contact.value.trim())) { showError(contact.id, "Enter a valid contact number."); valid = false; }

    if (!valid) return;

    await fetch("/api/Boats", {
    method: "POST",
    headers: {
        "Content-Type": "application/json"
    },
    body: JSON.stringify({
        boatName: boatName.value.trim(),
        ownerName: owner.value.trim(),
        registrationNumber: boatNumber.value.trim(),
        capacity: parseInt(contact.value)
    })
});

await renderEntries();

    confirmBanner.classList.add("show");
    confirmBanner.querySelector("strong").textContent = `${boatName.value.trim()} logged successfully.`;
    form.reset();
    confirmBanner.scrollIntoView({ behavior: "smooth", block: "center" });
  });
}

/* ---------- Fish Market sort ---------- */
function setupMarketSort() {
  const buttons = document.querySelectorAll(".sort-btns button");
  const tbody = document.getElementById("marketTableBody");
  if (!buttons.length || !tbody) return;

  buttons.forEach((btn) => {
    btn.addEventListener("click", () => {
      buttons.forEach((b) => b.classList.remove("active"));
      btn.classList.add("active");
      const key = btn.dataset.sort;
      const rows = Array.from(tbody.querySelectorAll("tr"));
      rows.sort((a, b) => {
        const valA = parseFloat(a.dataset[key]);
        const valB = parseFloat(b.dataset[key]);
        return key === "name" ? a.dataset.name.localeCompare(b.dataset.name) : valB - valA;
      });
      rows.forEach((r) => tbody.appendChild(r));
    });
  });
}

/* ---------- Dashboard count-up ---------- */
function setupCountUp() {
  const gauges = document.querySelectorAll("[data-countto]");
  if (!gauges.length) return;
  gauges.forEach((el) => {
    const target = parseFloat(el.dataset.countto);
    const decimals = el.dataset.decimals ? parseInt(el.dataset.decimals) : 0;
    const duration = 1100;
    const start = performance.now();
    function tick(now) {
      const progress = Math.min((now - start) / duration, 1);
      const value = target * progress;
      el.textContent = value.toFixed(decimals);
      if (progress < 1) requestAnimationFrame(tick);
    }
    requestAnimationFrame(tick);
  });
}

/* ---------- Contact form ---------- */
function setupContactForm() {
  const form = document.getElementById("contactForm");
  if (!form) return;
  const banner = document.getElementById("contactConfirm");
  form.addEventListener("submit", async (e) => {
    e.preventDefault();
    banner.classList.add("show");
    form.reset();
  });
}

document.addEventListener("DOMContentLoaded", () => {
  renderHeader();
  renderFooter();
  setupRegistrationForm();
  setupMarketSort();
  setupCountUp();
  setupContactForm();
});
