import { ReactElement, useState } from "react";
import { useContext } from "react";
import PrimaryButton from "../PrimaryButton/PrimaryButton";
import styles from "./LoginForm.module.css";
import PrimaryInput from "../PrimaryInput/PrimaryInput";
import WebSocketContext from "../../context/WebSocketContext";
import ApiResponseType from "../../types/ResponseType";
import ClientContext from "../../context/ClientContext";

const LoginForm = (): ReactElement => {
  const [login, setLogin] = useState<string>();
  const context = useContext(WebSocketContext.wsContext);
  const clientContext = useContext(ClientContext.Context);

  context.on("LoginUser_Receive", (status: ApiResponseType) => {
    if (status === ApiResponseType.LoginExists) {
      clientContext.onLoginError();
    } else {
      clientContext.onLoginSuccess(login);
    }
  });

  return (
    <div className={styles.login_layout}>
      <div className={styles.login_layout_wrapper}>
        <div className={styles.login_layout_input}>
          <h1 className={styles.login_layout_title}>Авторизация</h1>
          <div className={styles.login_layout_primary_input}>
            <PrimaryInput
              onChange={(value: string) => setLogin(value)}
              placeholder="Имя пользователя"
            />
          </div>
          <PrimaryButton
            onClick={(e) => context.invoke("LoginUser", login)}
            text="Войти"
          />
        </div>
      </div>
    </div>
  );
};

export default LoginForm;
