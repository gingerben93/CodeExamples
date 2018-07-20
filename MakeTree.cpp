void drawPolygonPrism(int n, float base, float height) {

	glPushMatrix();
	glRotatef(-90, 1.0, 0.0, 0.0);

	// Cylinder "Cover"
	glBegin(GL_QUAD_STRIP);
	glColor3f(83 / 255.0, 53 / 255.0, 10 / 255.0);
	for (int i = 0; i < 480; i += (360 / n)) {
        //degrees to radians
		float a = i * M_PI / 180;
		glVertex3f(base * cos(a), base * sin(a), 0.0);
		glVertex3f(base * cos(a), base * sin(a), height);
	}
	glEnd();
	glPopMatrix();
}

//min height is 1; base can be anything; recursive function until height is < 1;
void makeTree(float height, float base) {

	float angleIntensity = height/10;
	int branchNumber;
    
    //number of sides; base size; height size;
	drawPolygonPrism(10,base,height);
    //shift branches up
	glTranslatef(0.0, height, 0.0);
	height -= 1; 
	base -= base*0.3;

    //random branch number
	branchNumber = rand() % 3 + 1;

	for (int x = 0; x < branchNumber; x++) {
		if (height > 1) {
			glPushMatrix();
            //x and z rotation of tree branches
			glRotatef(((rand() % 60) - 30) * angleIntensity, 1, 0, 0);
			glRotatef(((rand() % 60) - 30) * angleIntensity, 0, 0, 1);
			makeTree(height, base);
			glPopMatrix();

		}
	}
}