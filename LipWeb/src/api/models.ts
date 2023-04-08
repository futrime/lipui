class ToothItemInfo {
  tooth!: string;
  version!: string;
  information!: {
    author: string;
    description: string;
    homepage: string;
    license: string;
    name: string;
  };
}
interface ToothItemResult {
  packages: ToothItemInfo[];
  message: string;
}
export { type ToothItemResult, ToothItemInfo };
