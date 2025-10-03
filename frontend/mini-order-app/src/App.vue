<script lang="ts" setup>
import { onMounted } from 'vue'

 const dataURl = async() => {
  const response = await fetch("http://localhost:5172/orders")
  const data = await response.json()
  console.log(data)

 }
 

 onMounted(() => {
  dataURl()
 }) 
</script>