<template>
  <div class="container">
    <h1>Order Management</h1>

    <div>
      <button @click="currentView = 'list'">View Orders</button>
      <button @click="currentView = 'form'">New Order</button>
    </div>

  
    <div v-if="currentView === 'list'">
      <h2>Orders</h2>
      <table border="1">
        <thead>
          <tr>
            <th>ID</th>
            <th>Client</th>
            <th>Total</th>
            <th>Date</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="order in orders" :key="order.id">
            <td>{{ order.id }}</td>
            <td>{{ order.client }}</td>
            <td>{{ order.total }}</td>
            <td>{{ new Date(order.date).toLocaleString() }}</td>
            <td>
              <button @click="fetchOrderDetails(order.id)">View</button>
              <button @click="deleteOrder(order.id)">Delete</button>
            </td>
          </tr>
        </tbody>
      </table>

      
      <div v-if="selectedOrder">
        <h3>Order Details:</h3>
        <p><strong>ID:</strong> {{ selectedOrder.id }}</p>
        <p><strong>Client:</strong> {{ selectedOrder.client }}</p>
        <p><strong>Total:</strong> {{ selectedOrder.total }}</p>
        <p><strong>Date:</strong> {{ new Date(selectedOrder.date).toLocaleString() }}</p>
      </div>
    </div>


    <div v-if="currentView === 'form'">
      <h2>Create New Order</h2>
      <form @submit.prevent="createOrder">
        <div>
          <label>Client:</label>
          <input type="text" v-model="newOrder.client" required />
        </div>
        <div>
          <label>Total:</label>
          <input type="number" v-model.number="newOrder.total" required />
        </div>
        <button type="submit">Create</button>
      </form>

      <p v-if="formMessage">{{ formMessage }}</p>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';

const API_URL = 'http://localhost:5172/orders';

const orders = ref([]);
const selectedOrder = ref(null);
const newOrder = ref({
  client: '',
  total: 0
});
const formMessage = ref('');
const currentView = ref('list'); 

// Load all orders
const fetchOrders = async () => {
  try {
    const res = await fetch(API_URL);
    const data = await res.json();
    orders.value = data;
  } catch (error) {
    console.error('Error loading orders:', error);
  }
};

// View order details
const fetchOrderDetails = async (id) => {
  try {
    const res = await fetch(`${API_URL}/${id}`);
    if (!res.ok) throw new Error('Not found');
    const data = await res.json();
    selectedOrder.value = data;
  } catch (error) {
    selectedOrder.value = null;
    console.error('Error fetching order:', error);
  }
};

// Create new order
const createOrder = async () => {
  formMessage.value = '';
  try {
    const res = await fetch(API_URL, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(newOrder.value),
    });

    if (res.ok) {
      formMessage.value = 'Order created!';
      newOrder.value.client = '';
      newOrder.value.total = 0;
      fetchOrders(); 
      currentView.value = 'list';
    } else {
      formMessage.value = 'Error creating order';
    }
  } catch (error) {
    formMessage.value = 'Error creating order';
    console.error(error);
  }
};


const deleteOrder = async (id) => {
  const confirmDelete = confirm('Are you sure you want to delete this order?');
  if (!confirmDelete) return;

  try {
    const res = await fetch(`${API_URL}/${id}`, {
      method: 'DELETE'
    });

    if (res.ok) {
      // Remove from local list
      orders.value = orders.value.filter(order => order.id !== id);
      if (selectedOrder.value?.id === id) {
        selectedOrder.value = null;
      }
    } else {
      console.error('Error deleting order');
    }
  } catch (error) {
    console.error('Error:', error);
  }
};

onMounted(() => {
  fetchOrders();
});
</script>


<style scoped>
.container {
  max-width: 800px;
  margin: auto;
  padding: 20px;
  font-family: sans-serif;
}
table {
  width: 100%;
  margin-top: 10px;
}
td, th {
  padding: 8px;
  text-align: left;
}
button {
  margin: 5px;
  cursor: pointer;
}
form div {
  margin: 10px 0;
}
</style>
