import React, { ReactElement, ReactNode } from "react";
import styles from "./Layout.module.css";

const Layout = ({ children }: { children: ReactNode }): ReactElement => {
  return <div className={styles.layout}>{children}</div>;
};

export default Layout;
