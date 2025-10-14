"use client";

import useEmblaCarousel from "embla-carousel-react";
import AutoScroll, { AutoScrollType } from "embla-carousel-auto-scroll";
import Image from "next/image";
import Link from "next/link";
import React, { useEffect, useState } from "react";
import "@/assets/styles/sliderResponse.scss";

export interface Item {
  id: string | number;
  name: string;
  school: string;
  quote: string;
  avatar?: string;
  moreHref?: string;
}

interface Props {
  feedbacks: Item[];
  /** tốc độ px/giây (mặc định 1 = 1px/ms ≈ 1000px/s; khuyến nghị 2–6) */
  speed?: number;
}

export default function SliderResponse({ feedbacks, speed = 3 }: Props) {
  const [isPlaying, setIsPlaying] = useState(false);

  const [emblaRef, emblaApi] = useEmblaCarousel(
    {
      align: "start",
      loop: true,
      dragFree: true, // để lướt mượt khi kéo tay
    },
    [
      AutoScroll({
        speed, // tốc độ chạy ngang
        startDelay: 0, // bắt đầu ngay
        playOnInit: true, // tự chạy khi mount
        stopOnMouseEnter: true, // hover tạm dừng
        stopOnInteraction: false, // kéo tay xong vẫn tiếp tục chạy
      }),
    ]
  );

  useEffect(() => {
    if (!emblaApi) return;
    const auto = emblaApi.plugins()?.autoScroll as AutoScrollType | undefined;
    if (!auto) return;

    setIsPlaying(auto.isPlaying());

    const onPlay = () => setIsPlaying(true);
    const onStop = () => setIsPlaying(false);
    const onReInit = () => setIsPlaying(auto.isPlaying());

    emblaApi.on("autoScroll:play", onPlay);
    emblaApi.on("autoScroll:stop", onStop);
    emblaApi.on("reInit", onReInit);

    return () => {
      emblaApi.off("autoScroll:play", onPlay);
      emblaApi.off("autoScroll:stop", onStop);
      emblaApi.off("reInit", onReInit);
    };
  }, [emblaApi]);

  return (
    <section className="embla">
      <div className="embla__viewport" ref={emblaRef}>
        <div className="embla__container">
          {feedbacks.map((item) => (
            <div className="embla__slide" key={item.id}>
              <div className="cardWrap">
                <article className="card">
                  <header className="card__head">
                    <div className="card__avatar">
                      {item.avatar && (
                        <Image
                          src={item.avatar}
                          alt={item.name}
                          fill
                          sizes="44px"
                        />
                      )}
                    </div>
                    <div className="card__meta">
                      <div className="card__name">{item.name}</div>
                      <div className="card__school">{item.school}</div>
                    </div>
                  </header>

                  <p className="card__quote">{item.quote}</p>

                  {item.moreHref && (
                    <Link className="card__more" href={item.moreHref}>
                      Xem thêm
                    </Link>
                  )}
                </article>
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* Nếu cần hiển thị trạng thái */}
      {/* <div className="embla__status">{isPlaying ? "Đang chạy" : "Tạm dừng"}</div> */}
    </section>
  );
}
