document.addEventListener("DOMContentLoaded", () => {
  const toast = document.getElementById("demoToast");
  let toastTimer;

  document.querySelectorAll("[data-demo-action]").forEach((element) => {
    element.addEventListener("click", (event) => {
      event.preventDefault();
      if (!toast) return;
      toast.textContent = element.dataset.demoAction;
      toast.classList.add("show");
      window.clearTimeout(toastTimer);
      toastTimer = window.setTimeout(() => toast.classList.remove("show"), 3200);
    });
  });

  const gradingForm = document.getElementById("gradingForm");
  if (!gradingForm) return;

  const scoreInputs = [...gradingForm.querySelectorAll("[data-score]")];
  const feedback = document.getElementById("Input_LecturerFeedback");
  const finalScore = document.getElementById("finalScore");
  const classification = document.getElementById("classification");
  const feedbackCount = document.getElementById("feedbackCount");
  const validityNote = document.getElementById("validityNote");
  const confirmButton = document.getElementById("confirmButton");

  const classify = (score) => {
    if (score >= 8.5) return "A - Giỏi";
    if (score >= 7) return "B - Khá";
    if (score >= 5.5) return "C - Trung bình";
    if (score >= 4) return "D - Yếu";
    return "F - Kém";
  };

  const refresh = () => {
    const values = scoreInputs.map((input) => Number.parseFloat(input.value));
    const scoresAreValid = values.length === 3 && values.every((value) => Number.isFinite(value) && value >= 0 && value <= 10);
    const feedbackLength = feedback ? feedback.value.trim().length : 0;

    if (feedbackCount) feedbackCount.textContent = feedbackLength.toString();

    if (scoresAreValid && finalScore && classification) {
      const result = values[0] * 0.2 + values[1] * 0.3 + values[2] * 0.5;
      finalScore.textContent = result.toFixed(2);
      classification.textContent = classify(result);
    } else if (finalScore && classification) {
      finalScore.textContent = "--";
      classification.textContent = "Chưa đủ điểm";
    }

    const ready = scoresAreValid && feedbackLength >= 30;
    if (confirmButton) confirmButton.disabled = !ready;
    if (validityNote) {
      validityNote.classList.toggle("valid", ready);
      validityNote.classList.toggle("invalid", !ready);
      validityNote.textContent = ready
        ? "✓ Các trường điểm hợp lệ và đáp ứng điều kiện xác nhận kết quả."
        : "Nhập đủ ba đầu điểm từ 0.0 đến 10.0 và nhận xét tối thiểu 30 ký tự để xác nhận.";
    }
  };

  scoreInputs.forEach((input) => input.addEventListener("input", refresh));
  feedback?.addEventListener("input", refresh);
  refresh();

  confirmButton?.addEventListener("click", (event) => {
    if (!window.confirm("Xác nhận chốt điểm? Sau thao tác này, phiếu chấm sẽ bị khóa và không thể chỉnh sửa.")) {
      event.preventDefault();
    }
  });
});
