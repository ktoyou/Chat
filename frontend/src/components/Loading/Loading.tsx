import { Oval } from "react-loading-icons";
import styles from "./Loading.module.css";

const Loading = () => {
  return (
    <div className={styles.loading_layout}>
      <Oval stroke="#ffffff" />
    </div>
  );
};

export default Loading;
