import HashLoader from "react-spinners/HashLoader";

const Loading = () => {
  return (
    <>
      <div
        style={{
          position: "fixed",
          top: "50%",
          left: "50%",
          translate: "-50%, -50%",
        }}
      >
        <HashLoader color="#2abce9" />
      </div>
    </>
  );
};
export default Loading;
