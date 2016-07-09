for i in $(seq 0 4); do
	convert -size 30x30 xc:transparent -fill white -draw "rectangle 0,0 29,29" -font Ubuntu-Bold -pointsize 15 -fill black -stroke Black -draw "text 5,20 'S$i'" -fill transparent -draw "rectangle 0,0 29,29" small_$i.png
	convert -size 50x50 xc:transparent -fill white -draw "rectangle 0,0 49,49" -font Ubuntu-Bold -pointsize 15 -fill black -stroke Black -draw "text 5,20 'M$i'" -fill transparent -draw "rectangle 0,0 49,49" medium_$i.png
	convert -size 80x80 xc:transparent -fill white -draw "rectangle 0,0 79,79" -font Ubuntu-Bold -pointsize 15 -fill black -stroke Black -draw "text 5,20 'L$i'" -fill transparent -draw "rectangle 0,0 79,79" large_$i.png
done
