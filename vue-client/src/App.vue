<script setup lang="ts">
import { ref } from 'vue';

const data = ref<any[]>([]);
const api = 'http://localhost:5182/WeatherForecast';
const abortController = ref<AbortController | null>(null);

const getData = async () => {
  let reader: ReadableStreamDefaultReader<Uint8Array> | undefined;
  
  try {
    abortController.value = new AbortController();
    const signal = abortController.value.signal;

    let response = await fetch(api, { signal })
    reader = response.body?.getReader();
    if (!reader) return;
    const decoder = new TextDecoder();
    while (true) {
      const { done, value } = await reader.read();
      if (done) break;
      var item = decoder.decode(value).replace(/\[|]/g, '').replace(/^,/, '');
      var parsedItem = JSON.parse(item);
      data.value.push(parsedItem)
    }
  } catch (error) {
    console.error(error);
  } finally {
    reader?.releaseLock();
  }
}

getData();

function formatDate(dateString: string) {
  const date = new Date(dateString)
  const day = String(date.getDate()).padStart(2, '0');
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const year = date.getFullYear();
  return `${day}/${month}/${year}`;
}

function cancelFetch() {
  if (abortController.value) {
    abortController.value.abort();
  }
};

</script>

<template>
  <div class="container">
    <button @click="cancelFetch">Cancel</button>
    <table>
      <thead>
        <tr>
          <th>Date</th>
          <th>TemperatureC</th>
          <th>TemperatureF</th>
          <th>Summary</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="item in data">
          <td>{{ formatDate(item.date) }}</td>
          <td>{{ item.temperatureC }}</td>
          <td>{{ item.temperatureF }}</td>
          <td>{{ item.summary }}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<style scoped>
.container {
  width: 100%;
}

th {
  width: 200px;
  text-align: left;
}
</style>
