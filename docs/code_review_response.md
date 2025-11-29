Se propone la siguiente interfaz y refactor (resumen):

- Introducir IProviderStore (persistencia) con operaciones atómicas.
- Introducir ISelectionPolicy: interface que toma request + providers y devuelve provider.
- Usar transactions y row-level locking en Postgres (SELECT ... FOR UPDATE) para evitar double-assign.
- Añadir validaciones (latitude/longitude ranges, assistance type allowed).
- Añadir circuit-breaker y retries con policy (Polly).
