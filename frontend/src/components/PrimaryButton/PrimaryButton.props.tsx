import { ButtonHTMLAttributes, MouseEventHandler } from "react";

interface IPrimaryButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  text?: string;
  onClick?: MouseEventHandler;
}

export default IPrimaryButtonProps;
