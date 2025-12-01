import React, { useState, useEffect } from "react";
import axios from "axios";

export default function OptimizeForm() {
  const [latitude, setLatitude] = useState("");
  const [longitude, setLongitude] = useState("");
  const [assistanceType, setAssistanceType] = useState("grua");
  const [rating, setRating] = useState(4.0);
  const [result, setResult] = useState(null);
  const [loading, setLoading] = useState(false);

  const API_BASE = process.env.REACT_APP_API_URL || "http://localhost:5000";

  useEffect(() => {
    // optional: try to get geolocation
    if (!("geolocation" in navigator)) return;
    navigator.geolocation.getCurrentPosition(
      (pos) => {
        setLatitude(pos.coords.latitude.toFixed(6));
        setLongitude(pos.coords.longitude.toFixed(6));
      },
      () => {},
      { enableHighAccuracy: false, maximumAge: 60000 }
    );
  }, []);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setResult(null);

    try {
      const payload = {
        name: "web-request",
        latitude: parseFloat(latitude),
        longitude: parseFloat(longitude),
        assistanceType,
        rating: parseFloat(rating),
        services: assistanceType
      };

      const response = await axios.post(`${API_BASE}/optimize`, payload);

      // Set initial response
      setResult({ ...response.data, tracking: [] });

      // --- TRACKING SSE ---
      const eventSource = new EventSource(`${API_BASE}/tracking/${response.data.requestId}`);

      eventSource.onmessage = (event) => {
        setResult(prev => ({
          ...prev,
          tracking: [...prev.tracking, event.data]
        }));
      };

      eventSource.onerror = () => {
        eventSource.close();
      };
    } catch (err) {
      console.error(err);
      const message = err?.response?.data || err.message || "Error al contactar la API";
      setResult({ error: String(message) });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="card" style={{maxWidth:420, margin:"0 auto"}}>
      <form onSubmit={handleSubmit}>
        <div className="row">
          <div className="col">
            <label>Latitud</label>
            <input type="number" step="0.000001" value={latitude} onChange={(e)=>setLatitude(e.target.value)} required />
          </div>
          <div className="col">
            <label>Longitud</label>
            <input type="number" step="0.000001" value={longitude} onChange={(e)=>setLongitude(e.target.value)} required />
          </div>
        </div>

        <div>
          <label>Tipo de asistencia</label>
          <select value={assistanceType} onChange={(e)=>setAssistanceType(e.target.value)}>
            <option value="grua">Grúa</option>
            <option value="cerrajeria">Cerrajería</option>
            <option value="bateria">Batería</option>
          </select>
        </div>

        <div>
          <label>Rating mínimo</label>
          <input type="number" min="0" max="5" step="0.1" value={rating} onChange={(e)=>setRating(e.target.value)} />
        </div>

        <div style={{display:"flex",gap:8}}>
          <button type="submit" disabled={loading}>{loading ? "Buscando..." : "Solicitar Asistencia"}</button>
        </div>
      </form>

      {result && (
  <div className="result" style={{padding: "20px"}}>

    {result.error ? (
                <>
                  <h3 style={{color:"#b91c1c"}}>❌ Error</h3>
                  <p>{result.error}</p>
                </>
              ) : (
                <>
                  <h3>Proveedor Encontrado</h3>

                  <div style={{
                    background:"#fff",
                    padding:"16px",
                    borderRadius:"10px",
                    border:"1px solid #e5e7eb",
                    boxShadow:"0 4px 10px rgba(0,0,0,0.05)"
                  }}>
                    <p><strong>Solicitud:</strong> {result.requestName}</p>
                    <p>
                      <strong>Ubicación solicitada:</strong>
                      <br/>
                      Lat: {result.latitude} • Lng: {result.longitude}
                    </p>
                    <p><strong>Rating solicitado:</strong> {result.rating}</p>

                    <hr style={{margin:"14px 0"}}/>

                    <h4 style={{margin:"6px 0"}}>{result.provider.name}</h4>
                    <p><strong>Rating:</strong> ⭐ {result.provider.rating}</p>

                    <p>
                      <strong>Servicios:</strong> {result.provider.services.join(", ")}
                    </p>

                    <p>
                      <strong>Ubicación del proveedor:</strong>
                      <br/>
                      Lat: {result.provider.latitude} • Lng: {result.provider.longitude}
                    </p>

                    <p>
                      <strong>Disponible:</strong>
                      {result.provider.isAvailable ? (
                        <span style={{color:"#059669"}}> ✔ Sí</span>
                      ) : (
                        <span style={{color:"#b91c1c"}}> ✘ No</span>
                      )}
                    </p>
                  </div>
                
                {/* 🔥 TRACKING EN TIEMPO REAL */}
              <h4>Seguimiento en tiempo real</h4>
              <ul>
                {result.tracking?.map((t, i) => (
                  <li key={i}>📡 {t}</li>
                ))}
              </ul>
             </>
              )}
            </div>
          )}
    </div>
  );
}
