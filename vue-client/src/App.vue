<script setup lang="ts">
import { ref } from 'vue';

const data = ref<any[]>([]);
const fetchningData = ref<boolean>(false);
const api = 'http://localhost:5182/WeatherForecast';
const abortController = ref<AbortController | null>(null);

async function getData() {
  let reader: ReadableStreamDefaultReader<Uint8Array> | undefined;
  data.value = [];
  fetchningData.value = true;
  abortController.value = new AbortController();

  try {
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
  } catch (error: any) {
    if (error instanceof DOMException && error.name === 'AbortError') {
      console.log('Fetch aborted');
    } else {
      console.error('Fetch error:', error);
    }
  } finally {
    reader?.releaseLock();
    fetchningData.value = false;
  }
}

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
    <div style="padding: 10px;">
      <v-btn v-if="fetchningData === false" @click="getData" color="primary">Fetch Data</v-btn>
      <v-btn v-if="fetchningData" @click="cancelFetch" color="error">Cancel</v-btn>
    </div>
    <v-table>
      <thead>
        <tr>
          <th colspan="4" style="height: 6px; padding: 0;">
            <v-progress-linear v-if="fetchningData" color="primary" height="3" indeterminate rounded>
            </v-progress-linear>
          </th>
        </tr>
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
    </v-table>
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

button{
  width: 120px;
}
</style>
