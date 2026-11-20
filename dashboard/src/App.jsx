
// import { useEffect, useState } from "react";
import { useEffect, useState, useRef } from "react";
import "./App.css";

import {
  ResponsiveContainer,
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend
} from "recharts";

const API_URL = "http://localhost:5247/api";

function App() {
  const [machines, setMachines] = useState([]);
  const [readings, setReadings] = useState([]);
  const [alerts, setAlerts] = useState([]);
  const [alertLogs, setAlertLogs] = useState([]);
  const [loading, setLoading] = useState(true);
  const [apiConnected, setApiConnected] = useState(false);
const [lastUpdated, setLastUpdated] = useState(null);
  const [selectedMachine, setSelectedMachine] = useState("all");
  const knownAlertIds = useRef(new Set());
  const isFirstAlertLoad = useRef(true);
  const [downtimes, setDowntimes] = useState([]);

  async function fetchDashboardData() {
    try {
      const [
        machineResponse,
        readingResponse,
        alertLogResponse,
        downtimeResponse
      ] = await Promise.all([
        fetch(`${API_URL}/Machines`),
        fetch(`${API_URL}/SensorReadings`),
        fetch(`${API_URL}/AlertLogs`),
  fetch(`${API_URL}/MachineDowntimes`)
      ]);

      if (
        !machineResponse.ok ||
        !readingResponse.ok ||
        !alertLogResponse.ok ||
  !downtimeResponse.ok
      ) {
        throw new Error("API request failed");
      }

      const machineData = await machineResponse.json();
      const readingData = await readingResponse.json();
      const alertLogData = await alertLogResponse.json();
      const downtimeData = await downtimeResponse.json();

      setApiConnected(true);
setLastUpdated(new Date());

// First load: existing alerts-ஐ மட்டும் track செய்யவும்
if (isFirstAlertLoad.current) {
  alertLogData.forEach((log) => {
    knownAlertIds.current.add(log.id);
  });

  isFirstAlertLoad.current = false;
} else {
  // Detect newly created alerts
  const newAlerts = alertLogData.filter(
    (log) => !knownAlertIds.current.has(log.id)
  );

  console.log("New Alerts Found:", newAlerts);
  // newAlerts.forEach((log) => {
  //   knownAlertIds.current.add(log.id);

  //   if ("Notification" in window && Notification.permission === "granted") {
  //     new Notification(`New ${log.severity} Alert`, {
  //       body: `${log.alertType}: ${log.message}`,
  //       tag: `alert-${log.id}`
  //     });
  //   }
  // });

  newAlerts.forEach((log) => {
  knownAlertIds.current.add(log.id);

  console.log("New alert detected:", log);

  if ("Notification" in window && Notification.permission === "granted") {
    const notification = new Notification(
      `New ${log.severity} Alert`,
      {
        body: `${log.alertType}: ${log.message}`,
        requireInteraction: true,
        tag: `alert-${log.id}`
      }
    );

    notification.onclick = () => {
      window.focus();
      notification.close();
    };
  }
});
}

setMachines(machineData);
setReadings(readingData);
setAlertLogs(alertLogData);
setDowntimes(downtimeData);
      // const alertLogData = await alertLogResponse.json();

      // setMachines(machineData);
      // setReadings(readingData);
      // setAlertLogs(alertLogData);

      const alertResults = await Promise.all(
        machineData.map(async (machine) => {
          const response = await fetch(
            `${API_URL}/MachineAlerts/${machine.id}`
          );

          if (!response.ok) return null;

          return response.json();
        })
      );

      setAlerts(alertResults.filter(Boolean));
    } catch (error) {
      console.error("Dashboard error:", error);
      setApiConnected(false);
    } finally {
      setLoading(false);
    }
  }

  async function acknowledgeAlert(alertId) {
    try {
      const response = await fetch(
        `${API_URL}/AlertLogs/${alertId}/acknowledge`,
        {
          method: "PUT"
        }
      );

      if (!response.ok) {
        throw new Error("Unable to acknowledge alert");
      }

      await fetchDashboardData();
    } catch (error) {
      console.error("Acknowledge error:", error);
      alert("Failed to acknowledge alert");
    }
  }


  // async function requestNotificationPermission() {
  // if ("Notification" in window && Notification.permission === "default") {
  //   await Notification.requestPermission();
  // }

  async function requestNotificationPermission() {
  if (!("Notification" in window)) {
    alert("This browser does not support notifications.");
    return;
  }

  const permission = await Notification.requestPermission();

  if (permission === "granted") {
    new Notification("Notifications Enabled", {
      body: "Industrial Machine Monitoring alerts are now enabled."
    });
  } else {
    alert("Please allow notifications in your browser settings.");
  }
// }

}

  useEffect(() => {
    fetchDashboardData();

    const interval = setInterval(fetchDashboardData, 5000);

    return () => clearInterval(interval);
  }, []);

  const filteredMachines =
    selectedMachine === "all"
      ? machines
      : machines.filter(
          (machine) => machine.id === Number(selectedMachine)
        );

  const filteredReadings =
    selectedMachine === "all"
      ? readings
      : readings.filter(
          (reading) => reading.machineId === Number(selectedMachine)
        );

  const filteredAlerts =
    selectedMachine === "all"
      ? alerts
      : alerts.filter(
          (alert) => alert.machineId === Number(selectedMachine)
        );

  const filteredAlertLogs =
    selectedMachine === "all"
      ? alertLogs
      : alertLogs.filter(
          (log) => log.machineId === Number(selectedMachine)
        );
  const filteredDowntimes =
  selectedMachine === "all"
    ? downtimes
    : downtimes.filter(
        (downtime) =>
          downtime.machineId === Number(selectedMachine)
      );

  const latestReading = filteredReadings[0] ?? null;

  // const runningMachines = filteredMachines.filter(
  //   (machine) => machine.machineStatus === "RUNNING"
  // ).length;

  const runningMachines = filteredMachines.filter((machine) => {
  const latestMachineReading = readings.find(
    (reading) => reading.machineId === machine.id
  );

  if (!latestMachineReading) return false;

  const lastReadingTime = new Date(
    latestMachineReading.recordedAt
  ).getTime();

  return Date.now() - lastReadingTime <= 15000;
}).length;
  const warningCount = filteredAlerts.filter(
    (alert) => alert.status === "WARNING"
  ).length;

  const pendingAlertCount = filteredAlertLogs.filter(
    (log) => !log.isAcknowledged
  ).length;

  const chartData = filteredReadings
    .slice(0, 20)
    .reverse()
    .map((reading) => ({
      time: new Date(reading.recordedAt).toLocaleTimeString(),
      temperature: reading.temperature,
      current: reading.motorCurrent,
      vibration: reading.vibration
    }));

  return (
    <div className="dashboard">
      <header className="header">
        <div>
          <h1>Industrial Monitoring System</h1>
          <p>Real-time machine monitoring dashboard</p>
        </div>

        {/* <div className="live-status">
          <span className="live-dot"></span>
          LIVE MONITORING
        </div> */}

        <div className={`live-status ${apiConnected ? "connected" : "disconnected"}`}>
  <span className="live-dot"></span>
  {apiConnected ? "API CONNECTED" : "API DISCONNECTED"}
</div>

<div className="last-updated">
  Last Updated:{" "}
  {lastUpdated
    ? lastUpdated.toLocaleTimeString()
    : "Waiting..."}
</div>
        <button onClick={requestNotificationPermission}>
          Enable Notifications
        </button>

        <div className="filter-container">
          <label htmlFor="machineFilter">Select Machine:</label>

          <select
            id="machineFilter"
            value={selectedMachine}
            onChange={(e) => setSelectedMachine(e.target.value)}
          >
            <option value="all">All Machines</option>

            {machines.map((machine) => (
              <option key={machine.id} value={machine.id}>
                {machine.machineName}
              </option>
            ))}
          </select>
        </div>
      </header>

      {loading && <p>Loading dashboard...</p>}

      <section className="cards">
        <div className="card">
          <h3>Total Machines</h3>
          <h2>{filteredMachines.length}</h2>
        </div>

        <div className="card">
          <h3>Running Machines</h3>
          <h2>{runningMachines}</h2>
        </div>

        <div className="card">
          <h3>Active Warnings</h3>
          <h2 className="warning">{warningCount}</h2>
        </div>

        <div className="card">
          <h3>Latest Temperature</h3>
          <h2>
            {latestReading
              ? `${latestReading.temperature} °C`
              : "--"}
          </h2>
        </div>
      </section>

      <section className="section">
        <h2>Machine Status</h2>

        <div className="machine-grid">
          {/* {filteredMachines.map((machine) => {
            const machineAlert = filteredAlerts.find(
              (alert) => alert.machineId === machine.id
            );

            return ( */}
            {filteredMachines.map((machine) => {
  const machineAlert = filteredAlerts.find(
    (alert) => alert.machineId === machine.id
  );

  const machineReadings = readings.filter(
    (reading) => reading.machineId === machine.id
  );

  // const latestMachineReading = machineReadings[0];
  const latestMachineReading = readings
  .filter((reading) => reading.machineId === machine.id)
  .sort(
    (a, b) =>
      new Date(b.recordedAt) - new Date(a.recordedAt)
  )[0];

  const isOffline =
    !latestMachineReading ||
    (Date.now() - new Date(latestMachineReading.recordedAt).getTime()) > 15000;

  const machineHealth = isOffline
    ? "OFFLINE"
    : machineAlert?.status ?? "UNKNOWN";

    

  return (
              <div className="machine-card" key={machine.id}>
                <h3>{machine.machineName}</h3>

                <p>Code: {machine.machineCode}</p>


<div className="machine-sensor-values">
  <div className="sensor-metric">
    <span>Temperature</span>
    <strong>
      {latestMachineReading
        ? `${latestMachineReading.temperature} °C`
        : "N/A"}
    </strong>
  </div>

  <div className="sensor-metric">
    <span>Motor Current</span>
    <strong>
      {latestMachineReading
        ? `${latestMachineReading.motorCurrent} A`
        : "N/A"}
    </strong>
  </div>

  <div className="sensor-metric">
    <span>Vibration</span>
    <strong>
      {latestMachineReading
        ? `${latestMachineReading.vibration} mm/s`
        : "N/A"}
    </strong>
  </div>
</div>


                {/* <p>Machine Status: {machine.machineStatus}</p> */}
                <p>
  Machine Status:{" "}
  <span className={isOffline ? "offline" : "normal"}>
    {isOffline ? "OFFLINE" : "RUNNING"}
  </span>
</p>
                <p>
                  Health:{" "}
                  {/* <span
                    className={
                      machineAlert?.status === "WARNING"
                        ? "warning"
                        : "normal"
                    }
                  >
                    {machineAlert?.status ?? "UNKNOWN"}
                  </span> */}
                  <span
    className={
      machineHealth === "WARNING" || machineHealth === "OFFLINE"
        ? "warning"
        : "normal"
    }
  >
    {machineHealth}
  </span>
                </p>

                {machineAlert?.alerts?.length > 0 && (
                  <div className="alert-box">
                    {machineAlert.alerts.map((alert, index) => (
                      <p key={index}>{alert}</p>
                    ))}
                  </div>
                )}
              </div>
            );
          })}
        </div>
      </section>

      <section className="section">
        <h2>Live Sensor Analytics</h2>

        <div className="chart-grid">
          <div className="chart-card">
            <h3>Temperature (°C)</h3>

            <ResponsiveContainer width="100%" height={280}>
              <LineChart data={chartData}>
                <CartesianGrid strokeDasharray="3 3" stroke="#334155" />
                <XAxis dataKey="time" stroke="#94a3b8" />
                <YAxis stroke="#94a3b8" />
                <Tooltip />
                <Legend />
                <Line
                  type="monotone"
                  dataKey="temperature"
                  stroke="#f97316"
                  strokeWidth={2}
                  dot={false}
                />
              </LineChart>
            </ResponsiveContainer>
          </div>

          <div className="chart-card">
            <h3>Motor Current (A)</h3>

            <ResponsiveContainer width="100%" height={280}>
              <LineChart data={chartData}>
                <CartesianGrid strokeDasharray="3 3" stroke="#334155" />
                <XAxis dataKey="time" stroke="#94a3b8" />
                <YAxis stroke="#94a3b8" />
                <Tooltip />
                <Legend />
                <Line
                  type="monotone"
                  dataKey="current"
                  stroke="#38bdf8"
                  strokeWidth={2}
                  dot={false}
                />
              </LineChart>
            </ResponsiveContainer>
          </div>

          <div className="chart-card">
            <h3>Vibration</h3>

            <ResponsiveContainer width="100%" height={280}>
              <LineChart data={chartData}>
                <CartesianGrid strokeDasharray="3 3" stroke="#334155" />
                <XAxis dataKey="time" stroke="#94a3b8" />
                <YAxis stroke="#94a3b8" />
                <Tooltip />
                <Legend />
                <Line
                  type="monotone"
                  dataKey="vibration"
                  stroke="#a78bfa"
                  strokeWidth={2}
                  dot={false}
                />
              </LineChart>
            </ResponsiveContainer>
          </div>
        </div>
      </section>

      <section className="section">
        <h2>Latest Sensor Readings</h2>

        <div className="table-container">
          <table>
            <thead>
              <tr>
                <th>Machine ID</th>
                <th>Temperature °C</th>
                <th>Motor Current A</th>
                <th>Vibration</th>
                <th>Recorded At</th>
              </tr>
            </thead>

            <tbody>
              {filteredReadings.slice(0, 10).map((reading) => (
                <tr key={reading.id}>
                  <td>{reading.machineId}</td>
                  <td>{reading.temperature}</td>
                  <td>{reading.motorCurrent}</td>
                  <td>{reading.vibration}</td>
                  <td>
                    {new Date(reading.recordedAt).toLocaleString()}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </section>

      {/* Alert History */}
      <section className="section">
        <h2>Alert History</h2>

        <p>
          Total Alerts: {filteredAlertLogs.length} | Pending:
          {" "}{pendingAlertCount}
        </p>

        <div className="table-container">
          <table>
            <thead>
              <tr>
                <th>Machine</th>
                <th>Alert Type</th>
                <th>Severity</th>
                <th>Message</th>
                <th>Actual Value</th>
                <th>Threshold</th>
                <th>Created At</th>
                <th>Status</th>
                <th>Action</th>
              </tr>
            </thead>

            <tbody>
              {filteredAlertLogs.map((log) => {
                const machine = machines.find(
                  (m) => m.id === log.machineId
                );

                return (
                  <tr key={log.id}>
                    <td>{machine?.machineName ?? `Machine ${log.machineId}`}</td>
                    <td>{log.alertType}</td>
                    <td>
                      {/* <span className={log.severity === "CRITICAL" ? "warning" : "warning"}>
                        {log.severity}
                      </span> */}
                      <span className={`severity-badge severity-${log.severity.toLowerCase()}`}>
                          {log.severity}
                      </span>
                    </td>
                    <td>{log.message}</td>
                    <td>{log.actualValue}</td>
                    <td>{log.thresholdValue}</td>
                    <td>
                      {new Date(log.createdAt).toLocaleString()}
                    </td>
                    <td>
                      {log.isAcknowledged ? "Acknowledged" : "Pending"}
                    </td>
                    <td>
                      {!log.isAcknowledged && (
                        <button
                          onClick={() => acknowledgeAlert(log.id)}
                        >
                          Acknowledge
                        </button>
                      )}
                    </td>
                  </tr>
                );
              })}

              {filteredAlertLogs.length === 0 && (
                <tr>
                  <td colSpan="9">No alert history available.</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </section>
      {/* Downtime History */}
<section className="section">
  <h2>Machine Downtime History</h2>

  <p>
    Total Downtime Records: {filteredDowntimes.length}
  </p>

  <div className="table-container">
    <table>
      <thead>
        <tr>
          <th>Machine</th>
          <th>Downtime Start</th>
          <th>Downtime End</th>
          <th>Duration (Minutes)</th>
          <th>Reason</th>
          <th>Status</th>
        </tr>
      </thead>

      <tbody>
        {filteredDowntimes.map((downtime) => (
          <tr key={downtime.id}>
            <td>
              {downtime.machineName ??
                `Machine ${downtime.machineId}`}
            </td>

            <td>
              {new Date(downtime.downtimeStart).toLocaleString()}
            </td>

            <td>
              {downtime.downtimeEnd
                ? new Date(downtime.downtimeEnd).toLocaleString()
                : "Still Offline"}
            </td>

            <td>
              {Number(downtime.durationMinutes).toFixed(2)}
            </td>

            <td>{downtime.reason}</td>

            <td>
              <span
                className={
                  downtime.isResolved
                    ? "normal"
                    : "warning"
                }
              >
                {downtime.isResolved
                  ? "RESOLVED"
                  : "ONGOING"}
              </span>
            </td>
          </tr>
        ))}

        {filteredDowntimes.length === 0 && (
          <tr>
            <td colSpan="6">
              No downtime history available.
            </td>
          </tr>
        )}
      </tbody>
    </table>
  </div>
</section>

      <footer>
        Data refreshes automatically every 5 seconds
      </footer>
    </div>
  );
}

export default App;


// import { useEffect, useState } from "react";
// import "./App.css";

// import {
//   ResponsiveContainer,
//   LineChart,
//   Line,
//   XAxis,
//   YAxis,
//   CartesianGrid,
//   Tooltip,
//   Legend
// } from "recharts";

// const API_URL = "http://localhost:5247/api";

// function App() {
//   const [machines, setMachines] = useState([]);
//   const [readings, setReadings] = useState([]);
//   const [alerts, setAlerts] = useState([]);
//   const [loading, setLoading] = useState(true);
//   const [selectedMachine, setSelectedMachine] = useState("all");

//   async function fetchDashboardData() {
//     try {
//       const machineResponse = await fetch(`${API_URL}/Machines`);
//       const readingResponse = await fetch(`${API_URL}/SensorReadings`);

//       if (!machineResponse.ok || !readingResponse.ok) {
//         throw new Error("API request failed");
//       }

//       const machineData = await machineResponse.json();
//       const readingData = await readingResponse.json();

//       setMachines(machineData);
//       setReadings(readingData);

//       const alertResults = await Promise.all(
//         machineData.map(async (machine) => {
//           const response = await fetch(
//             `${API_URL}/MachineAlerts/${machine.id}`
//           );

//           if (!response.ok) {
//             return null;
//           }

//           return response.json();
//         })
//       );

//       setAlerts(alertResults.filter(Boolean));
//       setLoading(false);
//     } catch (error) {
//       console.error("Dashboard error:", error);
//       setLoading(false);
//     }
//   }

//   useEffect(() => {
//     fetchDashboardData();

//     const interval = setInterval(fetchDashboardData, 5000);

//     return () => clearInterval(interval);
//   }, []);

//   // const latestReading = readings.length > 0 ? readings[0] : null;

//   // const runningMachines = machines.filter(
//   //   (machine) => machine.machineStatus === "RUNNING"
//   // ).length;

//   // const warningCount = alerts.filter(
//   //   (alert) => alert.status === "WARNING"
//   // ).length;


//   const filteredMachines =
//   selectedMachine === "all"
//     ? machines
//     : machines.filter(
//         (machine) => machine.id === Number(selectedMachine)
//       );

// const filteredReadings =
//   selectedMachine === "all"
//     ? readings
//     : readings.filter(
//         (reading) => reading.machineId === Number(selectedMachine)
//       );

// const filteredAlerts =
//   selectedMachine === "all"
//     ? alerts
//     : alerts.filter(
//         (alert) => alert.machineId === Number(selectedMachine)
//       );

// const latestReading = filteredReadings[0] ?? null;

// const runningMachines = filteredMachines.filter(
//   (machine) => machine.machineStatus === "RUNNING"
// ).length;

// const warningCount = filteredAlerts.filter(
//   (alert) => alert.status === "WARNING"
// ).length;

// const chartData = filteredReadings
//   .slice(0, 20)
//   .reverse()
//   .map((reading) => ({
//     time: new Date(reading.recordedAt).toLocaleTimeString(),
//     temperature: reading.temperature,
//     current: reading.motorCurrent,
//     vibration: reading.vibration
//   }));


//   // const chartData = readings
//   // .slice(0, 20)
//   // .reverse()
//   // .map((reading) => ({
//   //   time: new Date(reading.recordedAt).toLocaleTimeString(),
//   //   temperature: reading.temperature,
//   //   current: reading.motorCurrent,
//   //   vibration: reading.vibration
//   // }));

//   return (
//     <div className="dashboard">
//       <header className="header">
//         <div>
//           <h1>Industrial Monitoring System</h1>
//           <p>Real-time machine monitoring dashboard</p>
//         </div>

//         <div className="live-status">
//           <span className="live-dot"></span>
//           LIVE MONITORING
//         </div>

//         <div className="filter-container">
//   <label htmlFor="machineFilter">Select Machine:</label>

//   <select
//     id="machineFilter"
//     value={selectedMachine}
//     onChange={(e) => setSelectedMachine(e.target.value)}
//   >
//     <option value="all">All Machines</option>

//     {machines.map((machine) => (
//       <option key={machine.id} value={machine.id}>
//         {machine.machineName}
//       </option>
//     ))}
//   </select>
// </div>

//       </header>



//       {loading && <p>Loading dashboard...</p>}

//       <section className="cards">
//         <div className="card">
//           <h3>Total Machines</h3>
//           {/* <h2>{machines.length}</h2> */}
//           <h2>{filteredMachines.length}</h2>
//         </div>

//         <div className="card">
//           <h3>Running Machines</h3>
//           <h2>{runningMachines}</h2>
//         </div>

//         <div className="card">
//           <h3>Active Warnings</h3>
//           <h2 className="warning">{warningCount}</h2>
//         </div>

//         <div className="card">
//           <h3>Latest Temperature</h3>
//           <h2>
//             {latestReading
//               ? `${latestReading.temperature} °C`
//               : "--"}
//           </h2>
//         </div>
//       </section>

//       <section className="section">
//         <h2>Machine Status</h2>

//         <div className="machine-grid">
//           {/* {machines.map((machine) => { */}
//           {filteredMachines.map((machine) => {
//             // const machineAlert = alerts.find(
//             const machineAlert = filteredAlerts.find(
//               (alert) => alert.machineId === machine.id
//             );

//             return (
//               <div className="machine-card" key={machine.id}>
//                 <h3>{machine.machineName}</h3>

//                 <p>
//                   Code: {machine.machineCode}
//                 </p>

//                 <p>
//                   Machine Status: {machine.machineStatus}
//                 </p>

//                 <p>
//                   Health:{" "}
//                   <span
//                     className={
//                       machineAlert?.status === "WARNING"
//                         ? "warning"
//                         : "normal"
//                     }
//                   >
//                     {machineAlert?.status ?? "UNKNOWN"}
//                   </span>
//                 </p>

//                 {machineAlert?.alerts?.length > 0 && (
//                   <div className="alert-box">
//                     {machineAlert.alerts.map((alert, index) => (
//                       <p key={index}>{alert}</p>
//                     ))}
//                   </div>
//                 )}
//               </div>
//             );
//           })}
//         </div>
//       </section>

      
// <section className="section">
//   <h2>Live Sensor Analytics</h2>

//   <div className="chart-grid">

//     <div className="chart-card">
//       <h3>Temperature (°C)</h3>

//       <ResponsiveContainer width="100%" height={280}>
//         <LineChart data={chartData}>
//           <CartesianGrid strokeDasharray="3 3" stroke="#334155" />
//           <XAxis dataKey="time" stroke="#94a3b8" />
//           <YAxis stroke="#94a3b8" />
//           <Tooltip />
//           <Legend />

//           <Line
//             type="monotone"
//             dataKey="temperature"
//             stroke="#f97316"
//             strokeWidth={2}
//             dot={false}
//           />
//         </LineChart>
//       </ResponsiveContainer>
//     </div>

//     <div className="chart-card">
//       <h3>Motor Current (A)</h3>

//       <ResponsiveContainer width="100%" height={280}>
//         <LineChart data={chartData}>
//           <CartesianGrid strokeDasharray="3 3" stroke="#334155" />
//           <XAxis dataKey="time" stroke="#94a3b8" />
//           <YAxis stroke="#94a3b8" />
//           <Tooltip />
//           <Legend />

//           <Line
//             type="monotone"
//             dataKey="current"
//             stroke="#38bdf8"
//             strokeWidth={2}
//             dot={false}
//           />
//         </LineChart>
//       </ResponsiveContainer>
//     </div>

//     <div className="chart-card">
//       <h3>Vibration</h3>

//       <ResponsiveContainer width="100%" height={280}>
//         <LineChart data={chartData}>
//           <CartesianGrid strokeDasharray="3 3" stroke="#334155" />
//           <XAxis dataKey="time" stroke="#94a3b8" />
//           <YAxis stroke="#94a3b8" />
//           <Tooltip />
//           <Legend />

//           <Line
//             type="monotone"
//             dataKey="vibration"
//             stroke="#a78bfa"
//             strokeWidth={2}
//             dot={false}
//           />
//         </LineChart>
//       </ResponsiveContainer>
//     </div>

//   </div>
// </section>


//       <section className="section">
//         <h2>Latest Sensor Readings</h2>

//         <div className="table-container">
//           <table>
//             <thead>
//               <tr>
//                 <th>Machine ID</th>
//                 <th>Temperature °C</th>
//                 <th>Motor Current A</th>
//                 <th>Vibration</th>
//                 <th>Recorded At</th>
//               </tr>
//             </thead>

//             <tbody>
//               {/* {readings.slice(0, 10).map((reading) => ( */}
//               {filteredReadings.slice(0, 10).map((reading) => (
//                 <tr key={reading.id}>
//                   <td>{reading.machineId}</td>
//                   <td>{reading.temperature}</td>
//                   <td>{reading.motorCurrent}</td>
//                   <td>{reading.vibration}</td>
//                   <td>
//                     {new Date(reading.recordedAt).toLocaleString()}
//                   </td>
//                 </tr>
//               ))}
//             </tbody>
//           </table>
//         </div>
//       </section>

//       <footer>
//         Data refreshes automatically every 5 seconds
//       </footer>
//     </div>
//   );
// }

// export default App;
