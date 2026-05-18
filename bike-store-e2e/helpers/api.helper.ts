const API_URL = process.env.API_URL || 'http://localhost:5000/api';

interface RegisterPayload {
  email: string;
  password: string;
  displayName: string;
}

interface AuthResponse {
  email: string;
  displayName: string;
  token: string;
}

interface CreateProductPayload {
  name: string;
  description: string;
  price: number;
  brand: string;
  type: string;
  quantityInStock: number;
}

export async function registerUser(payload: RegisterPayload): Promise<AuthResponse> {
  const res = await fetch(`${API_URL}/Auth/register`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  });
  if (!res.ok && res.status !== 409) {
    throw new Error(`Register failed: ${res.status} ${await res.text()}`);
  }
  // If 409 (already exists), login instead
  if (res.status === 409) {
    return loginUser({ email: payload.email, password: payload.password });
  }
  return res.json();
}

export async function loginUser(payload: { email: string; password: string }): Promise<AuthResponse> {
  const res = await fetch(`${API_URL}/Auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  });
  if (!res.ok) {
    throw new Error(`Login failed: ${res.status} ${await res.text()}`);
  }
  return res.json();
}

export async function createProduct(token: string, product: CreateProductPayload): Promise<any> {
  const res = await fetch(`${API_URL}/Products`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(product),
  });
  if (!res.ok) {
    throw new Error(`Create product failed: ${res.status} ${await res.text()}`);
  }
  return res.json();
}

export async function deleteProduct(token: string, id: number): Promise<void> {
  const res = await fetch(`${API_URL}/Products/${id}`, {
    method: 'DELETE',
    headers: { Authorization: `Bearer ${token}` },
  });
  if (!res.ok && res.status !== 404) {
    throw new Error(`Delete product failed: ${res.status}`);
  }
}
