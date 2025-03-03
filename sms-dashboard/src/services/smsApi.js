import axios from "axios";

const api = axios.create({
    baseURL: "http://localhost:5167/api/v1", // Match your backend
});

export const fetchSmsLogs = async (filters) => {
    const response = await api.get("/Metrics/get-logs", { params: filters });
    return response.data;
};