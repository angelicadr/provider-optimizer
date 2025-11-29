import React from 'react';
import OptimizeForm from './OptimizeForm';

function App(){
  return (
    <div className="app">
      <header className="app-header">
        <h1>Solicitar  Asistencia</h1>
        <p className="subtitle">Solicita asistencia y encuentra el proveedor óptimo</p>
      </header>
      <main>
        <OptimizeForm />
      </main>
      <footer className="app-footer">
        <small>© Solicitar  Asistencia</small>
      </footer>
    </div>
  );
}

export default App;
