import { useState } from "react";
import { invoke } from "@tauri-apps/api/core";
import "./App.css";

type ScriptResult = {
  exitCode: number;
  stdout: string;
  stderr: string;
};

type OperationKey = "check" | "block" | "unblock";

const OPERATIONS: Record<
  OperationKey,
  { command: string; working: string; success: string }
> = {
  check: {
    command: "check_installation",
    working: "Locating Steam...",
    success: "Steam installation found",
  },
  block: {
    command: "block_steam",
    working: "Applying firewall rules...",
    success: "Steam is blocked",
  },
  unblock: {
    command: "unblock_steam",
    working: "Removing firewall rules...",
    success: "Steam is unblocked",
  },
};

function App() {
  const [busyOperation, setBusyOperation] = useState<OperationKey | null>(
    null,
  );
  const [statusText, setStatusText] = useState("Ready");
  const [statusIsError, setStatusIsError] = useState(false);
  const [details, setDetails] = useState("");

  async function runOperation(operation: OperationKey) {
    const { command, working, success } = OPERATIONS[operation];

    setBusyOperation(operation);
    setStatusIsError(false);
    setStatusText(working);
    setDetails("");

    try {
      const result = await invoke<ScriptResult>(command);
      if (result.exitCode === 0) {
        setStatusText(success);
        setStatusIsError(false);
        setDetails(result.stdout.trim());
      } else {
        setStatusText("Operation could not be completed");
        setStatusIsError(true);
        setDetails(
          (result.stderr.trim() || result.stdout.trim()) +
            `\n(exit code ${result.exitCode})`,
        );
      }
    } catch (error) {
      setStatusText("Unexpected error");
      setStatusIsError(true);
      setDetails(String(error));
    } finally {
      setBusyOperation(null);
    }
  }

  const isBusy = busyOperation !== null;

  return (
    <main className="container">
      <h1>Steam Blocker</h1>
      <p className="subtitle">
        Runs the shared PowerShell engine to manage Steam's Windows Firewall
        rules.
      </p>

      <div className="row">
        <button
          type="button"
          disabled={isBusy}
          onClick={() => runOperation("check")}
        >
          Check installation
        </button>
        <button
          type="button"
          disabled={isBusy}
          onClick={() => runOperation("block")}
        >
          Block Steam
        </button>
        <button
          type="button"
          disabled={isBusy}
          onClick={() => runOperation("unblock")}
        >
          Unblock Steam
        </button>
      </div>

      <section className="status" aria-live="polite">
        <p className={statusIsError ? "status-text error" : "status-text"}>
          {statusText}
        </p>
        {details && <pre className="details">{details}</pre>}
      </section>
    </main>
  );
}

export default App;
