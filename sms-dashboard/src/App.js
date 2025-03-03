import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import SmsLogsPage from "./pages/SmsLogsPage";

function App() {
    return (
        <Router>
            <Routes>
                <Route path="/" element={<SmsLogsPage />} />
            </Routes>
        </Router>
    );
}

export default App;