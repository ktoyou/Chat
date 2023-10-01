import { ReactNode } from "react"

interface IModalProps {
	children?: ReactNode,
	header?: string,
	onClose: Function
}

export default IModalProps